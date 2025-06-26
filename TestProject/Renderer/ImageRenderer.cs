using MathLibrary;
using MathLibrary.Tracing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;

namespace TestProject.Renderer;

public class ImageRenderer(RenderConfig config, Camera camera, IReadOnlyList<ISceneObject> sceneObjects)
{
    private readonly RenderConfig _config = config;
    private readonly IReadOnlyList<ISceneObject> _sceneObjects = sceneObjects;
    private readonly Camera _camera = camera;
    private int _pixelsRendered;
    private readonly Lock _lockObject = new();

    public void RenderScene()
    {
        Console.WriteLine($"\n--- Начало рендеринга сцены {_config.ImageWidth}x{_config.ImageHeight} ---");
        Console.WriteLine($"Настройки: {_config.SamplesPerPixel} сэмплов/пиксель, глубина {_config.MaxDepth} отскоков.");
        var stopwatch = Stopwatch.StartNew();
        using var image = new Image<Rgba32>(_config.ImageWidth, _config.ImageHeight);

        _pixelsRendered = 0;
        var totalPixels = _config.ImageWidth * _config.ImageHeight;
        DrawProgressBar(0, totalPixels);

        Parallel.For(0, _config.ImageHeight, y =>
        {
            for (var x = 0; x < _config.ImageWidth; x++)
            {
                var totalColor = Vector3.Zero;
                for (var s = 0; s < _config.SamplesPerPixel; s++)
                {
                    var u = (x + Random.Shared.NextSingle()) / (_config.ImageWidth - 1);
                    var v = (y + Random.Shared.NextSingle()) / (_config.ImageHeight - 1);
                    var ray = _camera.GetRay(u, v);
                    totalColor += TraceRay(ray, _config.MaxDepth);
                }
                var finalColor = SquareRoot(totalColor / _config.SamplesPerPixel);
                image[x, _config.ImageHeight - 1 - y] = new Rgba32(
                    (byte)(Math.Clamp(finalColor.X, 0, 1) * 255),
                    (byte)(Math.Clamp(finalColor.Y, 0, 1) * 255),
                    (byte)(Math.Clamp(finalColor.Z, 0, 1) * 255));
            }

            var pixelsDone = Interlocked.Add(ref _pixelsRendered, _config.ImageWidth);
            lock (_lockObject)
            {
                DrawProgressBar(pixelsDone, totalPixels);
            }
        });

        DrawProgressBar(totalPixels, totalPixels);
        Console.WriteLine();
        image.SaveAsPng(_config.OutputFileName);
        stopwatch.Stop();
        Console.WriteLine($"--- Рендеринг завершен за {stopwatch.Elapsed.TotalSeconds:F2} с. Файл сохранен: {_config.OutputFileName} ---");
    }

    private Vector3 TraceRay(Ray ray, int depth)
    {
        if (depth <= 0) return Vector3.Zero;

        HitInfo closestHit = default;
        var hasHit = false;
        var closestDistance = float.MaxValue;

        foreach (var obj in _sceneObjects)
        {
            if (obj.Intersect(ray, 0.001f, closestDistance, out var currentHit))
            {
                hasHit = true;
                closestDistance = currentHit.Distance;
                closestHit = currentHit;
            }
        }

        if (!hasHit) return Vector3.Zero;

        var material = closestHit.HitObject.Material;
        if (material.Emission.MagnitudeSquared > 0) return material.Emission;

        var hitPoint = ray.Origin + ray.Direction * closestHit.Distance;

        var w = 1.0f - closestHit.U - closestHit.V;
        var normal = Vector3.Normalize(w * closestHit.HitObject.NormalA + closestHit.U * closestHit.HitObject.NormalB + closestHit.HitObject.NormalC);
        var outwardNormal = Vector3.Dot(ray.Direction, normal) < 0 ? normal : -normal;

        Vector3 nextDirection;
        var attenuation = material.Albedo;

        if (material.Transparency > 0 && Random.Shared.NextSingle() < material.Transparency)
        {
            var isEntering = Vector3.Dot(ray.Direction, normal) < 0;
            var n1 = isEntering ? 1.0f : material.IndexOfRefraction;
            var n2 = isEntering ? material.IndexOfRefraction : 1.0f;
            var n_ratio = n1 / n2;
            var cos_theta = MathF.Min(Vector3.Dot(-ray.Direction, outwardNormal), 1.0f);
            var sin_theta_sq = n_ratio * n_ratio * (1.0f - cos_theta * cos_theta);

            var cannotRefract = sin_theta_sq > 1.0f;
            var reflectance = SchlickReflectance(cos_theta, n_ratio);

            nextDirection = cannotRefract || reflectance > Random.Shared.NextSingle()
                ? Vector3.Reflect(ray.Direction, outwardNormal)
                : Vector3.Refract(ray.Direction, outwardNormal, n_ratio);
            attenuation = Vector3.One;
        }
        else
        {
            var diffuseDir = GetHemisphereDirection(outwardNormal);
            var specularDir = Vector3.Reflect(ray.Direction, outwardNormal);
            var roughSpecularDir = Vector3.Normalize(Vector3.Lerp(specularDir, diffuseDir, material.Roughness * material.Roughness));
            nextDirection = Vector3.Lerp(diffuseDir, roughSpecularDir, material.Metallic);
        }

        var newRayOrigin = hitPoint + outwardNormal * 0.0001f;
        var newRay = new Ray(newRayOrigin, nextDirection);
        return attenuation * TraceRay(newRay, depth - 1);
    }

    private static float SchlickReflectance(float cosine, float refractionRatio)
    {
        var r0 = (1 - refractionRatio) / (1 + refractionRatio);
        r0 *= r0;
        return r0 + (1 - r0) * MathF.Pow(1 - cosine, 5);
    }

    private static void DrawProgressBar(int current, int total)
    {
        Console.CursorLeft = 0;
        var progress = (float)current / total;
        var barWidth = 50;
        var filledWidth = (int)(progress * barWidth);
        Console.Write("[");
        Console.Write(new string('#', filledWidth));
        Console.Write(new string('-', barWidth - filledWidth));
        Console.Write($"] {progress:P1}");
    }

    private static Vector3 SquareRoot(Vector3 c) => new(MathF.Sqrt(c.X), MathF.Sqrt(c.Y), MathF.Sqrt(c.Z));

    private static Vector3 GetHemisphereDirection(Vector3 normal)
    {
        var up = MathF.Abs(normal.X) > 0.9f ? Vector3.UnitY : Vector3.UnitX;
        var tangent = Vector3.Normalize(Vector3.Cross(up, normal));
        var bitangent = Vector3.Cross(normal, tangent);
        var r1 = Random.Shared.NextSingle();
        var r2 = Random.Shared.NextSingle();
        var r = MathF.Sqrt(r1);
        var phi = 2 * MathF.PI * r2;
        var x = r * MathF.Cos(phi);
        var y = r * MathF.Sin(phi);
        var z = MathF.Sqrt(Math.Max(0.0f, 1.0f - r1));
        return tangent * x + bitangent * y + normal * z;
    }
}
