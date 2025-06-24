using MathLibrary.Core;

namespace MathLibrary;

public readonly partial record struct Matrix4x4
{
    /// <summary>
    /// Получает определитель матрицы.
    /// </summary>
    public float Determinant
    {
        get
        {
            // Расчет по первой строке
            var det = M11 * (M22 * (M33 * M44 - M34 * M43) - M23 * (M32 * M44 - M34 * M42) + M24 * (M32 * M43 - M33 * M42));
            det -= M12 * (M21 * (M33 * M44 - M34 * M43) - M23 * (M31 * M44 - M34 * M41) + M24 * (M31 * M43 - M33 * M41));
            det += M13 * (M21 * (M32 * M44 - M34 * M42) - M22 * (M31 * M44 - M34 * M41) + M24 * (M31 * M42 - M32 * M41));
            det -= M14 * (M21 * (M32 * M43 - M33 * M42) - M22 * (M31 * M43 - M33 * M41) + M23 * (M31 * M42 - M32 * M41));
            return det;
        }
    }

    /// <summary>
    /// Проверяет, является ли матрица единичной.
    /// </summary>
    public bool IsIdentity => Equals(Identity);

    /// <summary>
    /// Получает вектор трансляции (перемещения) матрицы.
    /// </summary>
    public Vector3 Translation => new(M41, M42, M43);

    /// <summary>
    /// Транспонирует матрицу.
    /// </summary>
    /// <returns>Транспонированная матрица.</returns>
    public Matrix4x4 Transpose() => new(
        M11, M21, M31, M41,
        M12, M22, M32, M42,
        M13, M23, M33, M43,
        M14, M24, M34, M44);

    /// <summary>
    /// Пытается инвертировать матрицу.
    /// </summary>
    /// <param name="result">Обратная матрица, если операция успешна.</param>
    /// <returns>True, если матрица обратима, иначе false.</returns>
    public bool TryInvert(out Matrix4x4 result)
    {
        var det = Determinant;
        if (MathHelper.Approximately(det, 0f))
        {
            result = Zero;
            return false;
        }

        var invDet = 1.0f / det;
        // Расчет матрицы алгебраических дополнений и транспонирование
        result = new Matrix4x4(
             (M22 * (M33 * M44 - M34 * M43) - M23 * (M32 * M44 - M34 * M42) + M24 * (M32 * M43 - M33 * M42)) * invDet,
            -(M12 * (M33 * M44 - M34 * M43) - M13 * (M32 * M44 - M34 * M42) + M14 * (M32 * M43 - M33 * M42)) * invDet,
             (M12 * (M23 * M44 - M24 * M43) - M13 * (M22 * M44 - M24 * M42) + M14 * (M22 * M43 - M23 * M42)) * invDet,
            -(M12 * (M23 * M34 - M24 * M33) - M13 * (M22 * M34 - M24 * M32) + M14 * (M22 * M33 - M23 * M32)) * invDet,

            -(M21 * (M33 * M44 - M34 * M43) - M23 * (M31 * M44 - M34 * M41) + M24 * (M31 * M43 - M33 * M41)) * invDet,
             (M11 * (M33 * M44 - M34 * M43) - M13 * (M31 * M44 - M34 * M41) + M14 * (M31 * M43 - M33 * M41)) * invDet,
            -(M11 * (M23 * M44 - M24 * M43) - M13 * (M21 * M44 - M24 * M41) + M14 * (M21 * M43 - M23 * M41)) * invDet,
             (M11 * (M23 * M34 - M24 * M33) - M13 * (M21 * M34 - M24 * M31) + M14 * (M21 * M33 - M23 * M31)) * invDet,

             (M21 * (M32 * M44 - M34 * M42) - M22 * (M31 * M44 - M34 * M41) + M24 * (M31 * M42 - M32 * M41)) * invDet,
            -(M11 * (M32 * M44 - M34 * M42) - M12 * (M31 * M44 - M34 * M41) + M14 * (M31 * M42 - M32 * M41)) * invDet,
             (M11 * (M22 * M44 - M24 * M42) - M12 * (M21 * M44 - M24 * M41) + M14 * (M21 * M42 - M22 * M41)) * invDet,
            -(M11 * (M22 * M34 - M24 * M32) - M12 * (M21 * M34 - M24 * M31) + M14 * (M21 * M32 - M22 * M31)) * invDet,

            -(M21 * (M32 * M43 - M33 * M42) - M22 * (M31 * M43 - M33 * M41) + M23 * (M31 * M42 - M32 * M41)) * invDet,
             (M11 * (M32 * M43 - M33 * M42) - M12 * (M31 * M43 - M33 * M41) + M13 * (M31 * M42 - M32 * M41)) * invDet,
            -(M11 * (M22 * M43 - M23 * M42) - M12 * (M21 * M43 - M23 * M41) + M13 * (M21 * M42 - M22 * M41)) * invDet,
             (M11 * (M22 * M33 - M23 * M32) - M12 * (M21 * M33 - M23 * M31) + M13 * (M21 * M32 - M22 * M31)) * invDet
        );
        return true;
    }
}
