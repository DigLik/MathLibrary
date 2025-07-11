using System;
using System.Runtime.InteropServices;

namespace MathLibrary.Geometry;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct Vector3(float X, float Y, float Z)
{
    public float this[int i] => i switch
    {
        0 => X,
        1 => Y,
        2 => Z,
        _ => throw new IndexOutOfRangeException()
    };

    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3 operator *(Vector3 a, float s) => new(a.X * s, a.Y * s, a.Z * s);
    public static Vector3 operator *(float s, Vector3 a) => new(a.X * s, a.Y * s, a.Z * s);
    public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

    public static Vector3 Min(Vector3 a, Vector3 b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
    public static Vector3 Max(Vector3 a, Vector3 b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

    public float LengthSquared() => X * X + Y * Y + Z * Z;
    public float Length() => MathF.Sqrt(LengthSquared());

    public static Vector3 Normalize(Vector3 v)
    {
        float len = v.Length();
        return len > Constants.Epsilon ? v * (1.0f / len) : v;
    }

    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );
    }

    internal static Vector3 Dot(Vector3 finalRight, Vector3 normal)
    {
        return new Vector3(
            finalRight.X * normal.X + finalRight.Y * normal.Y + finalRight.Z * normal.Z,
            finalRight.X * normal.X + finalRight.Y * normal.Y + finalRight.Z * normal.Z,
            finalRight.X * normal.X + finalRight.Y * normal.Y + finalRight.Z * normal.Z
        );
    }
}
