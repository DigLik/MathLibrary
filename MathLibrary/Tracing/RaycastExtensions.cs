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
        // Идея в том, чтобы рассматривать 3D-бокс как пересечение трех "слоев" (slabs) -
        // по одному для каждой оси (X, Y, Z). Мы вычисляем, на каком расстоянии (t)
        // луч входит и выходит из каждого слоя.

        float tmin = 0.0f;
        float tmax = float.MaxValue;

        // Ось X
        if (Math.Abs(ray.Direction.X) < 1e-6f) // Луч параллелен плоскостям X
        {
            if (ray.Origin.X < box.Min.X || ray.Origin.X > box.Max.X)
            {
                distance = 0;
                return false; // Луч параллелен и находится вне бокса
            }
        }
        else
        {
            float t1 = (box.Min.X - ray.Origin.X) / ray.Direction.X;
            float t2 = (box.Max.X - ray.Origin.X) / ray.Direction.X;

            if (t1 > t2)
            {
                (t2, t1) = (t1, t2);
            } // Убедимся, что t1 - вход, t2 - выход

            tmin = Math.Max(tmin, t1);
            tmax = Math.Min(tmax, t2);
        }

        // Ось Y
        if (Math.Abs(ray.Direction.Y) < 1e-6f)
        {
            if (ray.Origin.Y < box.Min.Y || ray.Origin.Y > box.Max.Y)
            {
                distance = 0;
                return false;
            }
        }
        else
        {
            float t1 = (box.Min.Y - ray.Origin.Y) / ray.Direction.Y;
            float t2 = (box.Max.Y - ray.Origin.Y) / ray.Direction.Y;

            if (t1 > t2)
            {
                (t2, t1) = (t1, t2);
            }

            tmin = Math.Max(tmin, t1);
            tmax = Math.Min(tmax, t2);
        }

        // Ось Z
        if (Math.Abs(ray.Direction.Z) < 1e-6f)
        {
            if (ray.Origin.Z < box.Min.Z || ray.Origin.Z > box.Max.Z)
            {
                distance = 0;
                return false;
            }
        }
        else
        {
            float t1 = (box.Min.Z - ray.Origin.Z) / ray.Direction.Z;
            float t2 = (box.Max.Z - ray.Origin.Z) / ray.Direction.Z;

            if (t1 > t2)
            {
                (t2, t1) = (t1, t2);
            }

            tmin = Math.Max(tmin, t1);
            tmax = Math.Min(tmax, t2);
        }

        // Если tmin > tmax, то луч не пересекает бокс (интервалы не накладываются).
        // Если tmax < 0, то бокс находится позади луча.
        if (tmin > tmax || tmax < 0)
        {
            distance = 0;
            return false;
        }

        // Если tmin < 0, луч начинается внутри бокса. Расстояние до пересечения - 0.
        // Иначе расстояние равно tmin.
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

        // Если a близко к нулю, луч параллелен плоскости треугольника.
        if (a > -Epsilon && a < Epsilon)
            return false;

        float f = 1.0f / a;
        Vector3 s = ray.Origin - triangle.A;
        float u = f * Vector3.Dot(s, h);

        // Проверка барицентрической координаты u
        if (u < 0.0f || u > 1.0f)
            return false;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(ray.Direction, q);

        // Проверка барицентрической координаты v
        if (v < 0.0f || u + v > 1.0f)
            return false;

        // На этом этапе мы знаем, что есть пересечение. Вычисляем t, чтобы узнать, где.
        float t = f * Vector3.Dot(edge2, q);

        if (t > Epsilon) // Пересечение находится в направлении луча
        {
            distance = t;
            return true;
        }
        else // Пересечение находится позади луча
        {
            return false;
        }
    }
}
