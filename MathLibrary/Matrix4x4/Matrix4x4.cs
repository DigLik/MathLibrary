using System.Runtime.InteropServices;

namespace MathLibrary;

/// <summary>
/// Матрица 4x4.
/// Элементы хранятся в порядке row-major.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Matrix4x4(
    float M11, float M12, float M13, float M14,
    float M21, float M22, float M23, float M24,
    float M31, float M32, float M33, float M34,
    float M41, float M42, float M43, float M44)
{
    /// <summary>
    /// Создает единичную матрицу 4x4.
    /// </summary>
    public Matrix4x4() : this(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1)
    {
    }

    /// <summary>
    /// Создает матрицу 4x4 с заданными значениями.
    /// </summary>
    /// <param name="value"></param>
    public Matrix4x4(float value) : this(
        value, 0, 0, 0,
        0, value, 0, 0,
        0, 0, value, 0,
        0, 0, 0, value)
    {
    }

    /// <summary>
    /// Создает матрицу 4x4 из массива значений.
    /// </summary>
    /// <param name="values"></param>
    public Matrix4x4(in Span<float> values) : this(
        values[0], values[1], values[2], values[3],
        values[4], values[5], values[6], values[7],
        values[8], values[9], values[10], values[11],
        values[12], values[13], values[14], values[15])
    {
    }
}
