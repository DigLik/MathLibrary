using MathLibrary;
using MathLibrary.BVH;
using MathLibrary.Geometry;
using MathLibrary.Tracing;

namespace TestProject.Renderer;

/// <summary>
/// Представляет идеальную математическую сферу.
/// </summary>
public class SphereObject(Vector3 center, float radius, Material material) : ISceneObject
{
    public bool Intersect(Ray ray, float tMin, float tMax, out HitInfo hitInfo)
    {
        hitInfo = default;
        var oc = ray.Origin - center;
        var a = ray.Direction.MagnitudeSquared;
        var half_b = Vector3.Dot(oc, ray.Direction);
        var c = oc.MagnitudeSquared - radius * radius;
        var discriminant = half_b * half_b - a * c;

        if (discriminant < 0) return false;

        var sqrt_d = MathF.Sqrt(discriminant);
        var root = (-half_b - sqrt_d) / a;
        if (root < tMin || root > tMax)
        {
            root = (-half_b + sqrt_d) / a;
            if (root < tMin || root > tMax) return false;
        }

        var hitPoint = ray.Origin + ray.Direction * root;
        var outwardNormal = (hitPoint - center) / radius;

        // Создаем фиктивный MeshTriangle, чтобы передать материал и нормаль
        var dummyTriangle = new MeshTriangle(new Triangle(), outwardNormal, outwardNormal, outwardNormal, material);
        hitInfo = new HitInfo(dummyTriangle, root, 0, 0); // U,V не нужны для сферы
        return true;
    }
}

/// <summary>
/// Представляет сложный полигональный объект с BVH-ускорением.
/// </summary>
public class MeshObject(List<MeshTriangle> triangles) : ISceneObject
{
    private readonly BvhNode? _bvhRoot = BvhBuilder.Build(triangles);

    public bool Intersect(Ray ray, float tMin, float tMax, out HitInfo hitInfo)
    {
        if (BvhTracer.Intersect(_bvhRoot, ray, out hitInfo, tMax) && hitInfo.Distance > tMin)
            return true;
        hitInfo = default;
        return false;
    }
}
