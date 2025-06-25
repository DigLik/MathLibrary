using System.Runtime.InteropServices;

namespace MathLibrary;

/// <summary>
/// Двухмерный вектор.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Vector2(float X, float Y)
{
    /// <summary>
    /// Создает новый экземпляр <see cref="Vector2"/> с заданными координатами X и Y.
    /// </summary>
    public Vector2() : this(0, 0) { }

    /// <summary>
    /// Создает новый экземпляр <see cref="Vector2"/> с заданными координатами X и Y.
    /// </summary>
    /// <param name="value"></param>
    public Vector2(float value) : this(value, value) { }
}
