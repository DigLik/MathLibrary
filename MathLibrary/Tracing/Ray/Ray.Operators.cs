using System.Runtime.CompilerServices;

namespace MathLibrary.Tracing;

public readonly partial record struct Ray
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ray operator +(in Ray ray, in Vector3 offset)
        => new(ray.Origin + offset, ray.Direction);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ray operator -(in Ray ray, in Vector3 offset)
        => new(ray.Origin - offset, ray.Direction);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ray operator *(in Ray ray, float scale)
        => new(ray.Origin * scale, ray.Direction * scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ray operator *(in Ray ray, in Vector3 scale)
        => new(ray.Origin * scale, ray.Direction * scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ray operator /(in Ray ray, float scale)
        => new(ray.Origin / scale, ray.Direction / scale);
}
