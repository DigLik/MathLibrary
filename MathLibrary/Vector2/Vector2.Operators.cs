using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Vector2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator +(in Vector2 a, in Vector2 b) => new(a.X + b.X, a.Y + b.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator -(in Vector2 a, in Vector2 b) => new(a.X - b.X, a.Y - b.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator -(in Vector2 a) => new(-a.X, -a.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator *(in Vector2 a, float d) => new(a.X * d, a.Y * d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator *(float d, in Vector2 a) => a * d;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator /(in Vector2 a, float d) => new(a.X / d, a.Y / d);
}
