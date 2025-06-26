using MathLibrary.BVH;
using MathLibrary.Geometry;

namespace MathLibrary.Tracing;
public static class BvhTracer
{
    public static bool Intersect(BvhNode? root, Ray ray, out HitInfo hitInfo)
    {
        return Intersect(root, ray, out hitInfo, float.MaxValue);
    }

    /// <summary>
    /// Находит ближайшее пересечение луча, но не дальше указанного расстояния.
    /// </summary>
    public static bool Intersect(BvhNode? root, Ray ray, out HitInfo hitInfo, float maxDistance)
    {
        hitInfo = default;
        if (root == null) return false;

        // Используем переданное максимальное расстояние как начальное
        float closestDistance = maxDistance;
        bool hasHit = false;
        var stack = new Stack<BvhNode>();

        // Если луч вообще не пересекает корневой BBox, выходим сразу
        if (ray.TryIntersect(root.BoundingBox, out float rootBoxDist) && rootBoxDist < closestDistance)
        {
            stack.Push(root);
        }
        else
        {
            return false;
        }

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (node.IsLeaf)
            {
                foreach (var meshTriangle in node.Primitives!)
                {
                    if (ray.TryIntersect(meshTriangle, out float distance, out float u, out float v) && distance < closestDistance)
                    {
                        closestDistance = distance;
                        hitInfo = new HitInfo(meshTriangle, distance, u, v);
                        hasHit = true;
                    }
                }
            }
            else
            {
                float distLeft = float.MaxValue;
                float distRight = float.MaxValue;

                bool hitLeft = node.LeftChild != null && ray.TryIntersect(node.LeftChild.BoundingBox, out distLeft);
                bool hitRight = node.RightChild != null && ray.TryIntersect(node.RightChild.BoundingBox, out distRight);

                // Отсекаем ветки, которые находятся дальше, чем уже найденное пересечение
                if (distLeft >= closestDistance) hitLeft = false;
                if (distRight >= closestDistance) hitRight = false;

                if (hitLeft && hitRight)
                {
                    if (distLeft < distRight)
                    {
                        stack.Push(node.RightChild!);
                        stack.Push(node.LeftChild!);
                    }
                    else
                    {
                        stack.Push(node.LeftChild!);
                        stack.Push(node.RightChild!);
                    }
                }
                else if (hitLeft)
                {
                    stack.Push(node.LeftChild!);
                }
                else if (hitRight)
                {
                    stack.Push(node.RightChild!);
                }
            }
        }
        return hasHit;
    }
}
