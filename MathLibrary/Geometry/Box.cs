using System;

namespace MathLibrary.Geometry;

public readonly record struct Box(Vector3 Min, Vector3 Max)
{
    public static readonly Box Empty = new(
        new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
        new Vector3(float.MinValue, float.MinValue, float.MinValue));

    public Vector3 Center => (Min + Max) * 0.5f;
    public Vector3 Size => Max - Min;

    public float SurfaceArea()
    {
        Vector3 s = Size;
        if (s.X < 0 || s.Y < 0 || s.Z < 0) return 0.0f;
        return 2.0f * (s.X * s.Y + s.X * s.Z + s.Y * s.Z);
    }

    public int MaxExtentAxis()
    {
        Vector3 s = Size;
        if (s.X > s.Y && s.X > s.Z) return 0;
        return s.Y > s.Z ? 1 : 2;
    }

    public float Offset(Vector3 p, int axis)
    {
        float offset = p[axis] - Min[axis];
        float extent = Max[axis] - Min[axis];
        return extent > 0 ? offset / extent : 0;
    }

    public static Box Combine(Box a, Box b) => new(Vector3.Min(a.Min, b.Min), Vector3.Max(a.Max, b.Max));
    public static Box Combine(Box box, Vector3 point) => new(Vector3.Min(box.Min, point), Vector3.Max(box.Max, point));
}