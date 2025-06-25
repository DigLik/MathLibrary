using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Vector3
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator +(in Vector3 a, in Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator -(in Vector3 a, in Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator -(in Vector3 a) => new(-a.X, -a.Y, -a.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(in Vector3 a, float d) => new(a.X * d, a.Y * d, a.Z * d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(float d, in Vector3 a) => a * d;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(in Vector3 a, in Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator /(in Vector3 a, float d) => new(a.X / d, a.Y / d, a.Z / d);
}
