using System.Runtime.CompilerServices;

namespace MathLibrary;

public readonly partial record struct Quaternion
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion operator +(in Quaternion a, in Quaternion b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion operator *(in Quaternion a, float s) => new(a.X * s, a.Y * s, a.Z * s, a.W * s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion operator /(in Quaternion a, float s) => new(a.X / s, a.Y / s, a.Z / s, a.W / s);

    /// <summary>
    /// Комбинирует два вращения. Порядок важен: q2 * q1 означает сначала вращение q1, затем q2.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion operator *(in Quaternion q2, in Quaternion q1)
        => new(q2.W * q1.X + q2.X * q1.W + q2.Y * q1.Z - q2.Z * q1.Y,
               q2.W * q1.Y - q2.X * q1.Z + q2.Y * q1.W + q2.Z * q1.X,
               q2.W * q1.Z + q2.X * q1.Y - q2.Y * q1.X + q2.Z * q1.W,
               q2.W * q1.W - q2.X * q1.X - q2.Y * q1.Y - q2.Z * q1.Z);

    /// <summary>
    /// Вращает вектор с помощью кватерниона.
    /// </summary>
    public static Vector3 operator *(in Quaternion q, in Vector3 v)
    {
        Vector3 u = new(q.X, q.Y, q.Z);
        var s = q.W;
        return 2.0f * Vector3.Dot(u, v) * u
             + (s * s - Vector3.Dot(u, u)) * v
             + 2.0f * s * Vector3.Cross(u, v);
    }
}
