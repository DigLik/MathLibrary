namespace MathLibrary;

public readonly partial record struct Matrix4x4
{
    /// <summary>
    /// Умножает две матрицы.
    /// </summary>
    public static Matrix4x4 operator *(in Matrix4x4 a, in Matrix4x4 b)
    {
        return new Matrix4x4(
            a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41,
            a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42,
            a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43,
            a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44,

            a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41,
            a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42,
            a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43,
            a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44,

            a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41,
            a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42,
            a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43,
            a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44,

            a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41,
            a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42,
            a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43,
            a.M41 * b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44
        );
    }

    /// <summary>
    /// Трансформирует 3D-вектор (точку) с помощью матрицы.
    /// </summary>
    public static Vector3 operator *(in Matrix4x4 m, in Vector3 v)
    {
        // Предполагаем, что w = 1 для точки, и применяем перспективное деление
        var invW = 1.0f / (m.M14 * v.X + m.M24 * v.Y + m.M34 * v.Z + m.M44);
        return new Vector3(
            (m.M11 * v.X + m.M21 * v.Y + m.M31 * v.Z + m.M41) * invW,
            (m.M12 * v.X + m.M22 * v.Y + m.M32 * v.Z + m.M42) * invW,
            (m.M13 * v.X + m.M23 * v.Y + m.M33 * v.Z + m.M43) * invW
        );
    }
}
