using System.Runtime.InteropServices;

namespace MathLibrary.Geometry;

/// <summary>
/// Треугольник в трехмерном пространстве, определяемый тремя вершинами A, B и C.
/// </summary>
/// <param name="A">Вершина A треугольника.</param>
/// <param name="B">Вершина B треугольника.</param>
/// <param name="C">Вершина C треугольника.</param>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Triangle(Vector3 A, Vector3 B, Vector3 C);
