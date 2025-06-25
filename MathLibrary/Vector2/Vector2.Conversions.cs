using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Vector2
{
    /// <summary>
    /// Неявное преобразование из кортежа (float, float) в Vector2.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2(in (float X, float Y) tuple) => new(tuple.X, tuple.Y);

    /// <summary>
    /// Явное преобразование из Vector2 в кортеж (float, float).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator (float X, float Y)(in Vector2 v) => (v.X, v.Y);
}
