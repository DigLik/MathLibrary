using System.Collections.Generic;
using System.Numerics;

namespace MathLibrary.Geometry;

public static class SceneFactory
{
    public static List<Triangle> CreateQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, CpuMaterial material)
    {
        return
        [
            new Triangle(p0, p1, p2, material),
            new Triangle(p0, p2, p3, material)
        ];
    }

    public static List<Triangle> CreateQuad(Vector3 center, float size, CpuMaterial material, Vector3 normal = default)
    {
        normal = normal == default
            ? new Vector3(0, 1, 0)
            : Vector3.Normalize(normal);

        float halfSize = size / 2.0f;

        Vector3 right;
        Vector3 upInPlane;

        if (Math.Abs(normal.Y) > 0.999f)
        {
            right = new Vector3(1, 0, 0);
            upInPlane = Vector3.Normalize(Vector3.Cross(right, normal));
        }
        else
        {
            right = Vector3.Normalize(Vector3.Cross(normal, new Vector3(0, 1, 0)));
            upInPlane = Vector3.Normalize(Vector3.Cross(right, normal));
        }

        Vector3 p0 = center - right * halfSize - upInPlane * halfSize;
        Vector3 p1 = center + right * halfSize - upInPlane * halfSize;
        Vector3 p2 = center + right * halfSize + upInPlane * halfSize;
        Vector3 p3 = center - right * halfSize + upInPlane * halfSize;

        return CreateQuad(p0, p1, p2, p3, material);
    }

    public static List<Triangle> CreateQuad(
        Vector3 center, float width, float height, CpuMaterial material, Vector3 normal = default, Vector3 right = default)
    {
        normal = normal == default
            ? new Vector3(0, 1, 0)
            : Vector3.Normalize(normal);

        Vector3 finalRight;
        Vector3 finalUpInPlane;

        if (right == default)
        {
            finalRight = Math.Abs(normal.Y) > 0.999f
                ? new Vector3(1, 0, 0)
                : Vector3.Normalize(Vector3.Cross(normal, new Vector3(0, 1, 0)));
        }
        else
        {
            finalRight = Vector3.Normalize(right);

            finalRight = Vector3.Normalize(finalRight - Vector3.Dot(finalRight, normal) * normal);

            if (finalRight.LengthSquared() < Constants.Epsilon)
            {
                finalRight = Math.Abs(normal.Y) > 0.999f
                    ? new Vector3(1, 0, 0)
                    : Vector3.Normalize(Vector3.Cross(normal, new Vector3(0, 1, 0)));
            }
        }

        finalUpInPlane = Vector3.Normalize(Vector3.Cross(finalRight, normal));

        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;

        Vector3 p0 = center - finalRight * halfWidth - finalUpInPlane * halfHeight;
        Vector3 p1 = center + finalRight * halfWidth - finalUpInPlane * halfHeight;
        Vector3 p2 = center + finalRight * halfWidth + finalUpInPlane * halfHeight;
        Vector3 p3 = center - finalRight * halfWidth + finalUpInPlane * halfHeight;

        return CreateQuad(p0, p1, p2, p3, material);
    }
}
