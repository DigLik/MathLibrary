using System.Runtime.InteropServices;

namespace MathLibrary;

/// <summary>
/// Кватернион.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Quaternion(float X, float Y, float Z, float W)
{
    /// <summary>
    /// Создает новый экземпляр <see cref="Quaternion"/> с заданными координатами X, Y, Z и W.
    /// </summary>
    public Quaternion() : this(0, 0, 0, 1) { }

    /// <summary>
    /// Создает новый экземпляр <see cref="Quaternion"/> с заданным значением для всех координат X, Y, Z и W.
    /// </summary>
    /// <param name="value"></param>
    public Quaternion(float value) : this(value, value, value, value) { }

    /// <summary>
    /// Создает новый экземпляр <see cref="Quaternion"/> с заданными координатами X и Y, Z равным 0 и W равным 1.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public Quaternion(float x, float y, float z) : this(x, y, z, 1) { }

    /// <summary>
    /// Создает новый экземпляр <see cref="Quaternion"/> из трехмерного вектора и заданного значения W.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="w"></param>
    public Quaternion(Vector3 vector, float w) : this(vector.X, vector.Y, vector.Z, w) { }
}
