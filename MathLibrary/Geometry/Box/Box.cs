using System.Runtime.InteropServices;

namespace MathLibrary.Geometry;

/// <summary>
/// Представляет трехмерный прямоугольный параллелепипед (Box) с минимальной и максимальной точками.
/// </summary>
/// <param name="Min"></param>
/// <param name="Max"></param>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Box(Vector3 Min, Vector3 Max)
{
    /// <summary>
    /// Создает новый экземпляр <see cref="Box"/> с заданными минимальной и максимальной точками.
    /// </summary>
    public Box() : this(Vector3.Zero, Vector3.Zero) { }

    /// <summary>
    /// Создает новый экземпляр <see cref="Box"/> с заданными минимальной и максимальной точками.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public Box(Vector2 min, Vector2 max)
        : this(new Vector3(min.X, min.Y, 0), new Vector3(max.X, max.Y, 0)) { }
}
