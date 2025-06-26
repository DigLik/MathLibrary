using MathLibrary.Core;
using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Vector3
{
    /// <summary>
    /// Возвращает вектор, содержащий минимальные компоненты из двух векторов.
    /// </summary>
    public static Vector3 Min(in Vector3 a, in Vector3 b)
        => new(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y), MathF.Min(a.Z, b.Z));

    /// <summary>
    /// Возвращает вектор, содержащий максимальные компоненты из двух векторов.
    /// </summary>
    public static Vector3 Max(in Vector3 a, in Vector3 b)
        => new(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y), MathF.Max(a.Z, b.Z));

    public static Vector3 Normalize(in Vector3 vector)
    {
        var magnitude = vector.Magnitude;
        return !MathHelper.Approximately(magnitude, 0f)
            ? vector / magnitude
            : Zero;
    }

    /// <summary>
    /// Вычисляет скалярное произведение двух векторов.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Dot(in Vector3 a, in Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    /// <summary>
    /// Вычисляет векторное произведение двух векторов.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Cross(in Vector3 a, in Vector3 b)
        => new(a.Y * b.Z - a.Z * b.Y,
               a.Z * b.X - a.X * b.Z,
               a.X * b.Y - a.Y * b.X);

    /// <summary>
    /// Вычисляет расстояние между двумя точками.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance(in Vector3 a, in Vector3 b) => (a - b).Magnitude;

    /// <summary>
    /// Вычисляет квадрат расстояния между двумя точками.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DistanceSquared(in Vector3 a, in Vector3 b) => (a - b).MagnitudeSquared;

    /// <summary>
    /// Линейно интерполирует между двумя векторами.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Lerp(in Vector3 a, in Vector3 b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new Vector3(
            a.X + (b.X - a.X) * t,
            a.Y + (b.Y - a.Y) * t,
            a.Z + (b.Z - a.Z) * t);
    }

    /// <summary>
    /// Проецирует вектор на другой вектор.
    /// </summary>
    public static Vector3 Project(in Vector3 vector, in Vector3 onNormal)
    {
        var sqrMag = onNormal.MagnitudeSquared;
        if (MathHelper.Approximately(sqrMag, 0f)) return Zero;
        var dot = Dot(vector, onNormal);
        return onNormal * (dot / sqrMag);
    }

    /// <summary>
    /// Отражает вектор от плоскости, заданной нормалью.
    /// </summary>
    public static Vector3 Reflect(in Vector3 inDirection, in Vector3 inNormal)
    {
        var dot = Dot(inDirection, inNormal);
        return inDirection - 2f * dot * inNormal;
    }
}
