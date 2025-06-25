using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Vector3
{
    /// <summary>
    /// Неявное преобразование из кортежа (float, float, float) в Vector3.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector3(in (float X, float Y, float Z) tuple) => new(tuple.X, tuple.Y, tuple.Z);

    /// <summary>
    /// Явное преобразование из Vector3 в кортеж (float, float, float).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator (float X, float Y, float Z)(in Vector3 v) => (v.X, v.Y, v.Z);
}
