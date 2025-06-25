using System.Runtime.InteropServices;

namespace MathLibrary;

/// <summary>
/// Трехмерный вектор.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Vector3(float X, float Y, float Z)
{
    /// <summary>
    /// Создает новый экземпляр <see cref="Vector3"/> с заданными координатами X, Y и Z.
    /// </summary>
    public Vector3() : this(0, 0, 0) { }

    /// <summary>
    /// Создает новый экземпляр <see cref="Vector3"/> с заданным значением для всех координат X, Y и Z.
    /// </summary>
    /// <param name="value"></param>
    public Vector3(float value) : this(value, value, value) { }

    /// <summary>
    /// Создает новый экземпляр <see cref="Vector3"/> с заданными координатами X и Y, и Z равным 0.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Vector3(float x, float y) : this(x, y, 0) { }
}
