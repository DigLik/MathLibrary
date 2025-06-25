using System.Runtime.InteropServices;

namespace MathLibrary.Geometry;

/// <summary>
/// Треугольник в трехмерном пространстве, определяемый тремя вершинами A, B и C.
/// </summary>
/// <param name="A">Вершина A треугольника.</param>
/// <param name="B">Вершина B треугольника.</param>
/// <param name="C">Вершина C треугольника.</param>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Triangle(Vector3 A, Vector3 B, Vector3 C)
{
    /// <summary>
    /// Создает треугольник с вершинами в начале координат (0, 0, 0), (1, 0, 0) и (0, 1, 0).
    /// </summary>
    public Triangle() : this(Vector3.Zero, Vector3.UnitX, Vector3.UnitY) { }

    /// <summary>
    /// Создает треугольник с заданными вершинами A, B и C в трехмерном пространстве.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public Triangle(Vector2 a, Vector2 b, Vector2 c)
        : this(new Vector3(a.X, a.Y, 0), new Vector3(b.X, b.Y, 0), new Vector3(c.X, c.Y, 0)) { }
}
