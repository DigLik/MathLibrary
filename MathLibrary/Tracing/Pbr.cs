using MathLibrary.Geometry;

namespace MathLibrary.Tracing
{
    /// <summary>
    /// Представляет физически корректный PBR материал.
    /// </summary>
    public readonly record struct Material
    {
        public Vector3 Albedo { get; init; }
        public Vector3 Emission { get; init; }
        public float Metallic { get; init; }
        public float Roughness { get; init; }
        public float Transparency { get; init; }
        public float IndexOfRefraction { get; init; }

        public Material()
        {
            Albedo = Vector3.One * 0.8f;
            Emission = Vector3.Zero;
            Metallic = 0.0f;
            Roughness = 1.0f;
            Transparency = 0.0f;
            IndexOfRefraction = 1.0f;
        }
    }

    /// <summary>
    /// Представляет треугольник с PBR-материалом и нормалями вершин.
    /// </summary>
    public readonly record struct MeshTriangle(Triangle Geometry, Vector3 NormalA, Vector3 NormalB, Vector3 NormalC, Material Material);

    /// <summary>
    /// Хранит полную информацию о результате пересечения луча.
    /// </summary>
    public readonly record struct HitInfo(MeshTriangle HitObject, float Distance, float U, float V);
}