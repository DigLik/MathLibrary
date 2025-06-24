using MathLibrary.Core;

namespace MathLibrary;

public readonly partial record struct Vector3
{
    /// <summary>
    /// Возвращает квадрат длины вектора.
    /// </summary>
    /// <remarks>Это быстрая операция, полезная для сравнения длин векторов.</remarks>
    public float MagnitudeSquared => X * X + Y * Y + Z * Z;

    /// <summary>
    /// Возвращает длину (величину) вектора.
    /// </summary>
    /// <remarks>Это относительно дорогая операция (квадратный корень). Для сравнения длин используйте <see cref="MagnitudeSquared"/>.</remarks>
    public float Magnitude => MathF.Sqrt(MagnitudeSquared);

    /// <summary>
    /// Возвращает новый вектор, являющийся нормализованной версией данного.
    /// </summary>
    /// <returns>Нормализованный вектор.</returns>
    /// <exception cref="DivideByZeroException">Если длина вектора равна нулю.</exception>
    public Vector3 Normalize()
    {
        var mag = Magnitude;
        return MathHelper.Approximately(mag, 0f)
            ? throw new DivideByZeroException("Невозможно нормализовать нулевой вектор.")
            : this / mag;
    }

    /// <summary>
    /// Пытается нормализовать вектор.
    /// </summary>
    /// <param name="result">Нормализованный вектор, если операция успешна.</param>
    /// <returns>True, если нормализация прошла успешно, иначе false (если вектор нулевой).</returns>
    public bool TryNormalize(out Vector3 result)
    {
        var magSq = MagnitudeSquared;
        if (MathHelper.Approximately(magSq, 0f))
        {
            result = Zero;
            return false;
        }
        result = this / MathF.Sqrt(magSq);
        return true;
    }

    /// <summary>
    /// Сравнивает данный вектор с другим на приблизительное равенство.
    /// </summary>
    /// <param name="other">Другой вектор для сравнения.</param>
    /// <returns>True, если векторы приблизительно равны.</returns>
    public bool ApproximatelyEquals(in Vector3 other)
        => MathHelper.Approximately(X, other.X) &&
           MathHelper.Approximately(Y, other.Y) &&
           MathHelper.Approximately(Z, other.Z);
}
