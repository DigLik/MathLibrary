using MathLibrary.Geometry;

namespace MathLibrary.Tracing;
public static class RaycastExtensions
{
    /// <summary>
    /// Проверяет, пересекает ли луч осе-ориентированный параллелепипед (AABB).
    /// Использует оптимизированный алгоритм "Slab Test".
    /// </summary>
    /// <param name="ray">Луч для проверки.</param>
    /// <param name="box">Параллелепипед для проверки.</param>
    /// <param name="distance">Выходной параметр: расстояние от начала луча до точки пересечения.
    /// Если луч начинается внутри бокса, расстояние равно 0.</param>
    /// <returns>true, если есть пересечение, иначе false.</returns>
    public static bool TryIntersect(this Ray ray, Box box, out float distance)
    {
        float tmin = 0.0f;
        float tmax = float.MaxValue;

        // Ось X
        if (Math.Abs(ray.Direction.X) < 1e-6f) { if (ray.Origin.X < box.Min.X || ray.Origin.X > box.Max.X) { distance = 0; return false; } }
        else
        {
            float t1 = (box.Min.X - ray.Origin.X) / ray.Direction.X; float t2 = (box.Max.X - ray.Origin.X) / ray.Direction.X;
            if (t1 > t2) (t2, t1) = (t1, t2);
            tmin = Math.Max(tmin, t1); tmax = Math.Min(tmax, t2);
        }

        // Ось Y
        if (Math.Abs(ray.Direction.Y) < 1e-6f) { if (ray.Origin.Y < box.Min.Y || ray.Origin.Y > box.Max.Y) { distance = 0; return false; } }
        else
        {
            float t1 = (box.Min.Y - ray.Origin.Y) / ray.Direction.Y; float t2 = (box.Max.Y - ray.Origin.Y) / ray.Direction.Y;
            if (t1 > t2) (t2, t1) = (t1, t2);
            tmin = Math.Max(tmin, t1); tmax = Math.Min(tmax, t2);
        }

        // Ось Z
        if (Math.Abs(ray.Direction.Z) < 1e-6f) { if (ray.Origin.Z < box.Min.Z || ray.Origin.Z > box.Max.Z) { distance = 0; return false; } }
        else
        {
            float t1 = (box.Min.Z - ray.Origin.Z) / ray.Direction.Z; float t2 = (box.Max.Z - ray.Origin.Z) / ray.Direction.Z;
            if (t1 > t2) (t2, t1) = (t1, t2);
            tmin = Math.Max(tmin, t1); tmax = Math.Min(tmax, t2);
        }

        if (tmin > tmax || tmax < 0) { distance = 0; return false; }
        distance = tmin < 0 ? 0 : tmin;
        return true;
    }


    /// <summary>
    /// Проверяет, пересекает ли луч треугольник.
    /// Использует алгоритм Мёллера-Трумбора.
    /// </summary>
    /// <param name="ray">Луч для проверки.</param>
    /// <param name="triangle">Треугольник для проверки.</param>
    /// <param name="distance">Выходной параметр: расстояние от начала луча до точки пересечения.</param>
    /// <returns>true, если есть пересечение, иначе false.</returns>
    public static bool TryIntersect(this Ray ray, Triangle triangle, out float distance)
    {
        const float Epsilon = 1e-6f;
        distance = 0;
        Vector3 edge1 = triangle.B - triangle.A;
        Vector3 edge2 = triangle.C - triangle.A;
        Vector3 h = Vector3.Cross(ray.Direction, edge2);
        float a = Vector3.Dot(edge1, h);
        if (a > -Epsilon && a < Epsilon) return false;
        float f = 1.0f / a;
        Vector3 s = ray.Origin - triangle.A;
        float u = f * Vector3.Dot(s, h);
        if (u < 0.0f || u > 1.0f) return false;
        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(ray.Direction, q);
        if (v < 0.0f || u + v > 1.0f) return false;
        float t = f * Vector3.Dot(edge2, q);
        if (t > Epsilon) { distance = t; return true; }
        return false;
    }
}
