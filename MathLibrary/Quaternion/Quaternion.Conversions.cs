using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Quaternion
{
    /// <summary>
    /// Неявное преобразование из кортежа (float, float, float, float) в Quaternion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Quaternion((float X, float Y, float Z, float W) tuple) => new(tuple.X, tuple.Y, tuple.Z, tuple.W);
}
