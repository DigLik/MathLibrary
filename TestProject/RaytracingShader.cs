using ComputeSharp;
using static ComputeSharp.Hlsl;

namespace TestProject;

[ThreadGroupSize(DefaultThreadGroupSizes.XY)]
[GeneratedComputeShaderDescriptor]
public readonly partial struct RaytracingShader(
    ReadWriteTexture2D<float4> accumulationBuffer,
    ReadOnlyBuffer<RaytracingShader.LinearBvhNode> bvhNodes,
    ReadOnlyBuffer<RaytracingShader.SceneObject> sceneObjects,
    Float3 cameraOrigin,
    Float3 cameraDirection,
    Float3 cameraUp,
    float verticalFov,
    uint batchSize,
    int maxBounces,
    int tileOffsetX,
    int tileOffsetY,
    uint currentBatch
    ) : IComputeShader
{
    public void Execute()
    {
        int2 localCoords = ThreadIds.XY;
        int2 globalCoords = new(tileOffsetX + localCoords.X, tileOffsetY + localCoords.Y);
        int2 resolution = new(accumulationBuffer.Width, accumulationBuffer.Height);
        uint rngState = (uint)(globalCoords.Y * resolution.X + globalCoords.X) + currentBatch * 0x9E3779B9;

        Vector3 pixelColor = Vec3(0, 0, 0);

        for (uint i = 0; i < batchSize; i++)
        {
            float u_rand = RandomFloat(ref rngState);
            float v_rand = RandomFloat(ref rngState);
            Ray currentRay = GenerateCameraRay(globalCoords, resolution, u_rand, v_rand);
            Vector3 pathThroughput = Vec3(1.0f, 1.0f, 1.0f);
            bool isInsideObject = false;
            Material insideMaterial = new Material();

            for (int bounce = 0; bounce < maxBounces; bounce++)
            {
                RayHit hit = TraceRay(currentRay);

                if (hit.Distance == float.MaxValue)
                {
                    break;
                }

                if (isInsideObject)
                {
                    Vector3 color = Vec3(Max(0.00001f, insideMaterial.BaseColor.X), Max(0.00001f, insideMaterial.BaseColor.Y), Max(0.00001f, insideMaterial.BaseColor.Z));
                    Vector3 absorption = Mul(Log(color), -hit.Distance);
                    pathThroughput = Mul(pathThroughput, Exp(absorption));
                }

                // Если луч попал на источник света, добавляем его энергию и завершаем путь.
                if (hit.Mat.EmissionStrength > 0)
                {
                    pixelColor = Add(pixelColor, Mul(pathThroughput, Mul(hit.Mat.EmissionColor, hit.Mat.EmissionStrength)));
                    break;
                }

                // Русская рулетка для длинных путей
                if (bounce > 4)
                {
                    float survivalProbability = Max(pathThroughput.X, Max(pathThroughput.Y, pathThroughput.Z));
                    if (RandomFloat(ref rngState) > survivalProbability || survivalProbability < 0.0001f)
                    {
                        break;
                    }
                    pathThroughput = Mul(pathThroughput, 1.0f / survivalProbability);
                }

                BSDFSample sample = Scatter(currentRay, hit, ref rngState);

                if (IsBlack(sample.Color))
                {
                    break;
                }

                pathThroughput = Mul(pathThroughput, sample.Color);

                // --- ОБНОВЛЕНИЕ СОСТОЯНИЯ "ВНУТРИ/СНАРУЖИ" ---
                // Если мы попали в прозрачный материал...
                if (hit.Mat.Transmission > 0.0f)
                {
                    // и если луч преломился (а не отразился)...
                    // Нам нужен способ узнать, было ли это преломление. Добавим флаг в BSDFSample.
                    if (sample.WasRefracted) // <--- НУЖНО ДОБАВИТЬ ЭТОТ ФЛАГ
                    {
                        // Переключаем состояние: если были снаружи - заходим внутрь, и наоборот.
                        isInsideObject = !isInsideObject;
                    }
                }
                else // Попали в непрозрачный материал
                {
                    // Мы определенно вышли наружу.
                    isInsideObject = false;
                }

                // Обновляем материал среды, если мы теперь внутри
                if (isInsideObject)
                {
                    insideMaterial = hit.Mat;
                }


                // --- СМЕЩЕНИЕ И ПРОДОЛЖЕНИЕ ТРАССИРОВКИ ---
                // (Используйте вашу старую, работающую логику смещения)
                Vector3 offsetNormal;
                if (Dot(sample.Direction, hit.TangentFrame.N) < 0)
                {
                    offsetNormal = Mul(hit.TangentFrame.N, -1.0f);
                }
                else
                {
                    offsetNormal = hit.TangentFrame.N;
                }
                currentRay.Origin = Add(hit.Position, Mul(offsetNormal, 0.0001f));
                currentRay.Direction = sample.Direction;
            }
        }

        float4 currentColor = accumulationBuffer[globalCoords];
        accumulationBuffer[globalCoords] = new float4(
            currentColor.X + pixelColor.X,
            currentColor.Y + pixelColor.Y,
            currentColor.Z + pixelColor.Z,
            1.0f);
    }

    #region Основная логика PBR

    private BSDFSample Scatter(Ray r_in, RayHit hit, ref uint rngState)
    {
        // Инициализируем возвращаемую структуру
        BSDFSample sample = new BSDFSample();
        sample.Color = Vec3(0, 0, 0);       // Цвет по умолчанию (черный)
        sample.Direction = Vec3(0, 0, 0);   // Направление по умолчанию
        sample.WasRefracted = false;        // По умолчанию считаем, что было отражение

        Vector3 wo = Mul(r_in.Direction, -1.0f); // Направление от точки к "глазу", т.е. обратное направлению луча

        // --- ОБРАБОТКА ДИЭЛЕКТРИКОВ (СТЕКЛО, ВОДА и т.д.) ---
        if (hit.Mat.Transmission > 0.0f)
        {
            // Определяем отношение коэффициентов преломления.
            // hit.FrontFace == true означает, что луч входит в объект извне.
            float eta = hit.FrontFace ? (1.0f / hit.Mat.IOR) : hit.Mat.IOR;

            Vector3 unit_direction = Normalize(r_in.Direction);
            Vector3 normal = hit.TangentFrame.N; // Используем нормаль из TangentFrame (должна быть face-forwarded)

            float cos_theta = Min(Dot(Mul(unit_direction, -1.0f), normal), 1.0f);
            float sin_theta = Sqrt(1.0f - cos_theta * cos_theta);

            // Проверяем возможность полного внутреннего отражения (TIR)
            bool cannot_refract = (eta * sin_theta) > 1.0f;
            float reflectance;

            if (cannot_refract)
            {
                // Полное внутреннее отражение, отражается 100% света
                reflectance = 1.0f;
            }
            else
            {
                // Вычисляем коэффициент отражения по приближению Шлика (Fresnel)
                // Примечание: cos_for_fresnel нужно считать для угла в более плотной среде.
                // Если мы выходим из объекта (hit.FrontFace == false), нам нужно вычислить новый косинус для преломленного луча.
                float cos_for_fresnel = cos_theta;
                if (!hit.FrontFace)
                {
                    cos_for_fresnel = Sqrt(Max(0.0f, 1.0f - eta * eta * (1.0f - cos_theta * cos_theta)));
                }

                float r0 = (1.0f - hit.Mat.IOR) / (1.0f + hit.Mat.IOR);
                r0 = r0 * r0;
                reflectance = r0 + (1.0f - r0) * Pow(1.0f - cos_for_fresnel, 5);
            }

            // На основе вероятности `reflectance` решаем, отражать или преломлять луч
            if (RandomFloat(ref rngState) < reflectance)
            {
                // --- ОТРАЖЕНИЕ ---
                sample.Direction = Reflect(unit_direction, normal);
                sample.Color = Vec3(1.0f, 1.0f, 1.0f); // Отраженный свет не меняет цвет (белый)
                sample.WasRefracted = false; // Это было отражение
            }
            else
            {
                // --- ПРЕЛОМЛЕНИЕ ---
                Vector3 refractedDir = Refract(unit_direction, normal, eta);

                // Добавляем эффект "матовости" для шероховатого стекла
                if (hit.Mat.Roughness > 0.0f)
                {
                    // Это нефизическая, но правдоподобная модель размытия
                    sample.Direction = Normalize(Add(refractedDir, Mul(RandomUnitVector(ref rngState), hit.Mat.Roughness)));
                }
                else
                {
                    sample.Direction = refractedDir;
                }

                sample.Color = Vec3(1.0f, 1.0f, 1.0f); // Преломленный свет также белый. 
                                                       // Поглощение будет рассчитано в основном цикле.
                sample.WasRefracted = true; // Это было преломление
            }

            return sample; // Возвращаемся, так как материал обработан
        }


        // --- ОБРАБОТКА НЕПРОЗРАЧНЫХ МАТЕРИАЛОВ (МЕТАЛЛЫ И ДИФФУЗНЫЕ) ---
        float metallic = hit.Mat.Metallic;

        // F0 - базовый коэффициент отражения при взгляде перпендикулярно поверхности
        Vector3 f0 = Mix(Vec3(0.04f, 0.04f, 0.04f), hit.Mat.BaseColor, metallic);

        // С вероятностью `metallic` материал ведет себя как металл, иначе - как диэлектрик (диффузный)
        if (RandomFloat(ref rngState) < metallic)
        {
            // --- МЕТАЛЛИЧЕСКОЕ ЗЕРКАЛЬНОЕ ОТРАЖЕНИЕ (GGX) ---
            float roughness = hit.Mat.Roughness * hit.Mat.Roughness;
            float aspect = Sqrt(1.0f - hit.Mat.Anisotropic * 0.9f);
            float alpha_x = roughness / aspect;
            float alpha_y = roughness * aspect;

            // Сэмплируем нормаль микроповерхности
            Vector3 wm = SampleGGX(wo, hit.TangentFrame, alpha_x, alpha_y, ref rngState);
            // Отражаем луч относительно этой микронормали
            sample.Direction = Reflect(Mul(wo, -1.0f), wm);
            sample.Color = f0; // Цвет отражения определяется F0 (эффект Френеля)

            // Убедимся, что луч не уходит "под" поверхность из-за агрессивных микрограней
            if (Dot(sample.Direction, hit.TangentFrame.N) <= 0)
            {
                sample.Color = Vec3(0, 0, 0); // Если ушел под поверхность, обнуляем вклад
            }
        }
        else
        {
            // --- ДИФФУЗНОЕ ОТРАЖЕНИЕ (ЛАМБЕРТ) ---
            // Идеальное диффузное отражение (Ламберт) сэмплируется по косинусу.
            // Простой вариант - сэмплировать по полусфере и умножать на cos(theta), 
            // но importance sampling лучше.
            // `Normalize(Add(hit.TangentFrame.N, RandomUnitVector(ref rngState)))` - это importance sampling для Ламберта.
            sample.Direction = Normalize(Add(hit.TangentFrame.N, RandomUnitVector(ref rngState)));
            sample.Color = hit.Mat.BaseColor; // Для диффузного отражения цвет - это просто базовый цвет материала
        }

        // `sample.WasRefracted` остается `false` для всех непрозрачных материалов
        return sample;
    }

    private TangentFrame CreateTangentFrame(Vector3 normal, Vector3 tangent)
    {
        TangentFrame frame;
        frame.N = normal;
        if (LengthSquared(tangent) < 0.0001f || Abs(Dot(normal, Normalize(tangent))) > 0.999f)
        {
            Vector3 other;
            if (Abs(normal.X) > 0.9f) { other = Vec3(0, 1, 0); } else { other = Vec3(1, 0, 0); }
            frame.T = Normalize(Sub(other, Mul(normal, Dot(other, normal))));
        }
        else
        {
            frame.T = Normalize(Sub(tangent, Mul(normal, Dot(tangent, normal))));
        }
        frame.B = Cross(frame.N, frame.T);
        return frame;
    }

    private Vector3 SampleGGX(Vector3 wo, TangentFrame frame, float alpha_x, float alpha_y, ref uint state)
    {
        Vector3 v = Normalize(Vec3(Dot(wo, frame.T), Dot(wo, frame.B), Dot(wo, frame.N)));
        Vector3 v_stretched = Normalize(Vec3(v.X * alpha_x, v.Y * alpha_y, v.Z));
        Vector3 t1;
        if (v_stretched.Z < 0.9999f) { t1 = Normalize(Cross(Vec3(0, 0, 1), v_stretched)); } else { t1 = Vec3(1, 0, 0); }
        Vector3 t2 = Cross(v_stretched, t1);
        float r = Sqrt(RandomFloat(ref state));
        float phi = 2.0f * PI * RandomFloat(ref state);
        float x = r * Cos(phi);
        float y = r * Sin(phi);
        float z = Sqrt(Max(0.0f, 1.0f - x * x - y * y));
        Vector3 h_stretched = Add(Add(Mul(t1, x), Mul(t2, y)), Mul(v_stretched, z));
        Vector3 wm_local = Normalize(Vec3(h_stretched.X * alpha_x, h_stretched.Y * alpha_y, Max(0.0f, h_stretched.Z)));
        return Add(Add(Mul(frame.T, wm_local.X), Mul(frame.B, wm_local.Y)), Mul(frame.N, wm_local.Z));
    }

    #endregion

    #region Вспомогательные методы и структуры

    private const float PI = 3.1415926535f;

    private Ray GenerateCameraRay(int2 pixelCoords, int2 resolution, float u_rand, float v_rand)
    {
        float aspectRatio = (float)resolution.X / resolution.Y;
        float fovAngle = Radians(verticalFov);
        float viewportHeight = 2.0f * Tan(fovAngle * 0.5f);
        float viewportWidth = viewportHeight * aspectRatio;
        Vector3 w = Normalize(Vec3(cameraDirection.X, cameraDirection.Y, cameraDirection.Z));
        Vector3 u_axis = Normalize(Cross(Vec3(cameraUp.X, cameraUp.Y, cameraUp.Z), w));
        Vector3 v_axis = Cross(w, u_axis);
        float px = (pixelCoords.X + u_rand) / resolution.X - 0.5f;
        float py = (pixelCoords.Y + v_rand) / resolution.Y - 0.5f;
        Vector3 rayDirection = Add(Add(Mul(u_axis, px * viewportWidth), Mul(v_axis, -py * viewportHeight)), w);
        Ray ray;
        ray.Origin = Vec3(cameraOrigin.X, cameraOrigin.Y, cameraOrigin.Z);
        ray.Direction = Normalize(rayDirection);
        return ray;
    }

    private RayHit TraceRay(Ray ray)
    {
        RayHit closestHit = new RayHit(); closestHit.Distance = float.MaxValue;
        TraversalStack stack = new TraversalStack(); stack.Ptr = 0;
        TraversalStack.Push(ref stack, 0);
        while (stack.Ptr > 0)
        {
            int nodeIndex = TraversalStack.Pop(ref stack); if (nodeIndex < 0) continue;
            LinearBvhNode node = bvhNodes[nodeIndex];
            if (!IntersectBox(ray, node.BoundingBox, 0.0001f, closestHit.Distance)) { continue; }
            if (node.PrimitiveCount > 0) { for (int i = 0; i < node.PrimitiveCount; i++) { int objIndex = node.FirstPrimitiveIndex + i; IntersectObject(ray, sceneObjects[objIndex], ref closestHit); } }
            else { TraversalStack.Push(ref stack, node.RightChildIndex); TraversalStack.Push(ref stack, node.LeftChildIndex); }
        }
        if (closestHit.Distance < float.MaxValue)
        {
            closestHit.Position = Add(ray.Origin, Mul(ray.Direction, closestHit.Distance));
            closestHit.FrontFace = Dot(ray.Direction, closestHit.Normal) < 0;
            if (!closestHit.FrontFace) closestHit.Normal = Mul(closestHit.Normal, -1);
            closestHit.ObjectTangent = new Vector3();
            closestHit.TangentFrame = CreateTangentFrame(closestHit.Normal, closestHit.ObjectTangent);
        }
        return closestHit;
    }

    private static void IntersectObject(Ray ray, SceneObject obj, ref RayHit closestHit)
    {
        if (obj.Type == 1) IntersectSphere(ray, obj, ref closestHit);
        else if (obj.Type == 3) IntersectTriangle(ray, obj, ref closestHit);
    }
    private static bool IntersectBox(Ray ray, Box box, float tMin, float tMax)
    {
        Vector3 invDir = Vec3(1.0f / ray.Direction.X, 1.0f / ray.Direction.Y, 1.0f / ray.Direction.Z);
        Vector3 t0s = Mul(Sub(box.Min, ray.Origin), invDir);
        Vector3 t1s = Mul(Sub(box.Max, ray.Origin), invDir);
        Vector3 t0 = Vec3(Min(t0s.X, t1s.X), Min(t0s.Y, t1s.Y), Min(t0s.Z, t1s.Z));
        Vector3 t1 = Vec3(Max(t0s.X, t1s.X), Max(t0s.Y, t1s.Y), Max(t0s.Z, t1s.Z));
        float tmin = Max(Max(t0.X, t0.Y), t0.Z);
        float tmax = Min(Min(t1.X, t1.Y), t1.Z);
        return tmax >= tmin && tmax > tMin && tmin < tMax;
    }
    private static void IntersectSphere(Ray ray, SceneObject obj, ref RayHit closestHit)
    {
        Vector3 oc = Sub(ray.Origin, obj.SphereCenter);
        float a = Dot(ray.Direction, ray.Direction);
        float b = 2.0f * Dot(oc, ray.Direction);
        float c = Dot(oc, oc) - obj.SphereRadius * obj.SphereRadius;
        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0) return;
        float sqrtD = Sqrt(discriminant);
        float t = (-b - sqrtD) / (2.0f * a);
        if (t < 0.0001f || t >= closestHit.Distance)
        {
            t = (-b + sqrtD) / (2.0f * a);
            if (t < 0.0001f || t >= closestHit.Distance) return;
        }
        closestHit.Distance = t;
        Vector3 hitPoint = Add(ray.Origin, Mul(ray.Direction, t));
        closestHit.Normal = Normalize(Sub(hitPoint, obj.SphereCenter));
        closestHit.Mat = obj.Mat;
    }
    private static void IntersectTriangle(Ray ray, SceneObject obj, ref RayHit closestHit)
    {
        const float Epsilon = 0.0000001f;
        Vector3 edge1 = Sub(obj.TriV1, obj.TriV0);
        Vector3 edge2 = Sub(obj.TriV2, obj.TriV0);
        Vector3 rayCrossE2 = Cross(ray.Direction, edge2);
        float det = Dot(edge1, rayCrossE2);
        if (det > -Epsilon && det < Epsilon) return;
        float invDet = 1.0f / det;
        Vector3 s = Sub(ray.Origin, obj.TriV0);
        float u = invDet * Dot(s, rayCrossE2);
        if (u < 0 || u > 1) return;
        Vector3 sCrossE1 = Cross(s, edge1);
        float v = invDet * Dot(ray.Direction, sCrossE1);
        if (v < 0 || u + v > 1) return;
        float t = invDet * Dot(edge2, sCrossE1);
        if (t > Epsilon && t < closestHit.Distance)
        {
            closestHit.Distance = t;
            closestHit.Normal = Normalize(Cross(edge1, edge2));
            closestHit.Mat = obj.Mat;
        }
    }

    private static uint RandomUInt(ref uint state)
    {
        state = state * 747796405u + 2891336453u;
        uint word = ((state >> (int)((state >> 28) + 4)) ^ state) * 277803737u;
        return (word >> 22) ^ word;
    }
    private static float RandomFloat(ref uint state) => (float)RandomUInt(ref state) / uint.MaxValue;
    private static Vector3 RandomInUnitSphere(ref uint state)
    {
        while (true)
        {
            Vector3 p = Vec3(RandomFloat(ref state) * 2.0f - 1.0f, RandomFloat(ref state) * 2.0f - 1.0f, RandomFloat(ref state) * 2.0f - 1.0f);
            if (LengthSquared(p) < 1.0f) return p;
        }
    }
    private static Vector3 RandomUnitVector(ref uint state) => Normalize(RandomInUnitSphere(ref state));

    private static Vector3 Reflect(Vector3 v, Vector3 n)
    {
        return Sub(v, Mul(n, 2 * Dot(v, n)));
    }

    private static Vector3 Refract(Vector3 uv, Vector3 n, float etai_over_etat)
    {
        float cos_theta = Min(Dot(Mul(uv, -1), n), 1.0f);
        Vector3 r_out_perp = Mul(Add(uv, Mul(n, cos_theta)), etai_over_etat);
        Vector3 r_out_parallel = Mul(n, -Sqrt(Max(0.0f, 1.0f - LengthSquared(r_out_perp))));
        return Add(r_out_perp, r_out_parallel);
    }

    private static Vector3 Vec3(float x, float y, float z) { Vector3 v; v.X = x; v.Y = y; v.Z = z; return v; }
    private static Vector3 Exp(Vector3 v) { return Vec3(Hlsl.Exp(v.X), Hlsl.Exp(v.Y), Hlsl.Exp(v.Z)); }
    private static Vector3 Log(Vector3 v) { return Vec3(Hlsl.Log(v.X), Hlsl.Log(v.Y), Hlsl.Log(v.Z)); }

    private static Vector3 Add(Vector3 a, Vector3 b) => Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    private static Vector3 Sub(Vector3 a, Vector3 b) => Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    private static Vector3 Mul(Vector3 a, float s) => Vec3(a.X * s, a.Y * s, a.Z * s);
    private static Vector3 Mul(Vector3 a, Vector3 b) => Vec3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    private static float Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    private static Vector3 Cross(Vector3 a, Vector3 b) => Vec3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
    private static float LengthSquared(Vector3 v) => Dot(v, v);
    private static float Length(Vector3 v) => Sqrt(Dot(v, v));
    private static Vector3 Normalize(Vector3 v)
    {
        float len = Length(v);
        if (len > 0.00001f)
            return Vec3(v.X / len, v.Y / len, v.Z / len);
        return v;
    }
    private static bool IsBlack(Vector3 v)
    {
        return v.X < 0.0001f && v.Y < 0.0001f && v.Z < 0.0001f;
    }
    private static Vector3 Mix(Vector3 a, Vector3 b, float t)
    {
        return Add(Mul(a, 1.0f - t), Mul(b, t));
    }

    #endregion

    #region Structures
    public struct Vector3
    {
        public float X, Y, Z;
    }
    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;
    }
    public struct Material
    {
        public Vector3 BaseColor;
        public float Metallic;
        public float Roughness;
        public float Transmission;
        public float IOR;
        public float Anisotropic;
        public float Clearcoat;
        public float ClearcoatRoughness;
        public Vector3 EmissionColor;
        public float EmissionStrength;
    }
    public struct RayHit
    {
        public float Distance;
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 ObjectTangent;
        public TangentFrame TangentFrame;
        public Bool FrontFace;
        public Material Mat;
    }
    public struct BSDFSample
    {
        public Vector3 Color;
        public Vector3 Direction;
        public Bool WasRefracted;
    }
    public struct TangentFrame
    {
        public Vector3 N, T, B;
    }
    public struct Box
    {
        public Vector3 Min;
        public Vector3 Max;
    }
    public struct LinearBvhNode
    {
        public Box BoundingBox;
        public int LeftChildIndex;
        public int RightChildIndex;
        public int PrimitiveCount;
        public int FirstPrimitiveIndex;
    }
    public struct SceneObject
    {
        public int Type;
        public Material Mat;
        public Vector3 Tangent;
        public Vector3 SphereCenter;
        public float SphereRadius;
        public Vector3 TriV0;
        public Vector3 TriV1;
        public Vector3 TriV2;
    }
    public struct TraversalStack
    {
        public int Ptr;
        public int N0, N1, N2, N3, N4, N5, N6, N7;
        public int N8, N9, N10, N11, N12, N13, N14, N15;
        public int N16, N17, N18, N19, N20, N21, N22, N23;
        public int N24, N25, N26, N27, N28, N29, N30, N31;

        public static void Push(ref TraversalStack stack, int value)
        {
            if (stack.Ptr >= 32) return;
            switch (stack.Ptr)
            {
                case 0: stack.N0 = value; break;
                case 1: stack.N1 = value; break;
                case 2: stack.N2 = value; break;
                case 3: stack.N3 = value; break;
                case 4: stack.N4 = value; break;
                case 5: stack.N5 = value; break;
                case 6: stack.N6 = value; break;
                case 7: stack.N7 = value; break;
                case 8: stack.N8 = value; break;
                case 9: stack.N9 = value; break;
                case 10: stack.N10 = value; break;
                case 11: stack.N11 = value; break;
                case 12: stack.N12 = value; break;
                case 13: stack.N13 = value; break;
                case 14: stack.N14 = value; break;
                case 15: stack.N15 = value; break;
                case 16: stack.N16 = value; break;
                case 17: stack.N17 = value; break;
                case 18: stack.N18 = value; break;
                case 19: stack.N19 = value; break;
                case 20: stack.N20 = value; break;
                case 21: stack.N21 = value; break;
                case 22: stack.N22 = value; break;
                case 23: stack.N23 = value; break;
                case 24: stack.N24 = value; break;
                case 25: stack.N25 = value; break;
                case 26: stack.N26 = value; break;
                case 27: stack.N27 = value; break;
                case 28: stack.N28 = value; break;
                case 29: stack.N29 = value; break;
                case 30: stack.N30 = value; break;
                case 31: stack.N31 = value; break;
            }
            stack.Ptr++;
        }

        public static int Pop(ref TraversalStack stack)
        {
            if (stack.Ptr <= 0) return -1;
            stack.Ptr--;
            switch (stack.Ptr)
            {
                case 0: return stack.N0;
                case 1: return stack.N1;
                case 2: return stack.N2;
                case 3: return stack.N3;
                case 4: return stack.N4;
                case 5: return stack.N5;
                case 6: return stack.N6;
                case 7: return stack.N7;
                case 8: return stack.N8;
                case 9: return stack.N9;
                case 10: return stack.N10;
                case 11: return stack.N11;
                case 12: return stack.N12;
                case 13: return stack.N13;
                case 14: return stack.N14;
                case 15: return stack.N15;
                case 16: return stack.N16;
                case 17: return stack.N17;
                case 18: return stack.N18;
                case 19: return stack.N19;
                case 20: return stack.N20;
                case 21: return stack.N21;
                case 22: return stack.N22;
                case 23: return stack.N23;
                case 24: return stack.N24;
                case 25: return stack.N25;
                case 26: return stack.N26;
                case 27: return stack.N27;
                case 28: return stack.N28;
                case 29: return stack.N29;
                case 30: return stack.N30;
                case 31: return stack.N31;
            }
            return -1;
        }
    }
    #endregion
}