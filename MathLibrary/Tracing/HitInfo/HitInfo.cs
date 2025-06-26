using MathLibrary.Geometry;

namespace MathLibrary.Tracing;

/// <summary>
/// Хранит информацию о результате пересечения луча с объектом.
/// </summary>
/// <param name="HitTriangle">Треугольник, с которым произошло пересечение.</param>
/// <param name="Distance">Расстояние от начала луча до точки пересечения.</param>
public readonly record struct HitInfo(
    MeshTriangle HitObject,
    float Distance,
    float U, // Барицентрическая координата U
    float V  // Барицентрическая координата V
);
