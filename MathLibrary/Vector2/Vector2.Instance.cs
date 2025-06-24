using MathLibrary.Core;

namespace MathLibrary;

public readonly partial record struct Vector2
{
    /// <summary>
    /// Возвращает квадрат длины вектора. Более производительно, чем Magnitude.
    /// </summary>
    public float MagnitudeSquared => X * X + Y * Y;

    /// <summary>
    /// Возвращает длину (величину) вектора.
    /// </summary>
    public float Magnitude => MathF.Sqrt(MagnitudeSquared);

    /// <summary>
    /// Возвращает новый вектор, являющийся нормализованной версией данного.
    /// </summary>
    /// <exception cref="DivideByZeroException">Если длина вектора равна нулю.</exception>
    public Vector2 Normalize()
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
    public bool TryNormalize(out Vector2 result)
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
    public bool ApproximatelyEquals(in Vector2 other)
        => MathHelper.Approximately(X, other.X) &&
           MathHelper.Approximately(Y, other.Y);
}
