using MathLibrary.Geometry;

namespace MathLibrary.Tracing;

/// <summary>
/// "Обертка" над Triangle, хранящая нормали для гладкого затенения.
/// </summary>
public readonly record struct MeshTriangle(
    Triangle Geometry,
    Vector3 NormalA,
    Vector3 NormalB,
    Vector3 NormalC
);
