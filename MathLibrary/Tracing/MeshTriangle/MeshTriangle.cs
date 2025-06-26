using MathLibrary.Geometry;

namespace MathLibrary.Tracing;

/// <summary>
/// Представляет треугольник с нормалями в каждой вершине для гладкого затенения.
/// Это основной примитив для рендеринга.
/// </summary>
public readonly record struct MeshTriangle(
    Triangle Geometry,
    Vector3 NormalA,
    Vector3 NormalB,
    Vector3 NormalC
);
