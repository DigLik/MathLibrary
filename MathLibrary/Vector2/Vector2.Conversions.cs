using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Vector2
{
    /// <summary>
    /// Неявное преобразование из кортежа (float, float) в Vector2.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2((float X, float Y) tuple) => new(tuple.X, tuple.Y);

    /// <summary>
    /// Явное преобразование Vector2 в Vector3. Компонента Z устанавливается в 0.
    /// </summary>
    public static explicit operator Vector3(in Vector2 v) => new(v.X, v.Y, 0);

    /// <summary>
    /// Явное преобразование Vector2 в Quaternion. Компоненты Z и W устанавливаются в 0.
    /// </summary>
    public static explicit operator Quaternion(in Vector2 v) => new(v.X, v.Y, 0, 0);
}
