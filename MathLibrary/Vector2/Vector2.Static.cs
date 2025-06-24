using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Vector2
{
    /// <summary>
    /// Вычисляет скалярное произведение двух векторов.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Dot(in Vector2 a, in Vector2 b) => a.X * b.X + a.Y * b.Y;

    /// <summary>
    /// Вычисляет расстояние между двумя точками.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance(in Vector2 a, in Vector2 b) => (a - b).Magnitude;

    /// <summary>
    /// Вычисляет квадрат расстояния между двумя точками.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DistanceSquared(in Vector2 a, in Vector2 b) => (a - b).MagnitudeSquared;

    /// <summary>
    /// Линейно интерполирует между двумя векторами.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Lerp(in Vector2 a, in Vector2 b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new Vector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
    }
}
