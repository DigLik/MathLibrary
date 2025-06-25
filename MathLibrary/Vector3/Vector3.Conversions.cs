using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Vector3
{
    /// <summary>
    /// Неявное преобразование из кортежа (float, float, float) в Vector3.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector3((float X, float Y, float Z) tuple) => new(tuple.X, tuple.Y, tuple.Z);

    /// <summary>
    /// Неявное преобразование из кортежа (float) в Vector3.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector3(float value) => new(value, value, value);
}
