using MathLibrary.Geometry;

/// <summary>
/// Хранит информацию о результате пересечения луча с объектом.
/// </summary>
/// <param name="HitTriangle">Треугольник, с которым произошло пересечение.</param>
/// <param name="Distance">Расстояние от начала луча до точки пересечения.</param>
public readonly record struct HitInfo(Triangle HitTriangle, float Distance);
