using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Matrix4x4
{
    /// <summary>
    /// Размер структуры в байтах.
    /// </summary>
    public const int SizeInBytes = 16 * sizeof(float);

    /// <summary>
    /// Нулевая матрица.
    /// </summary>
    public static Matrix4x4 Zero => new(
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0);

    /// <summary>
    /// Единичная матрица.
    /// </summary>
    public static Matrix4x4 Identity => new(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1);
}
