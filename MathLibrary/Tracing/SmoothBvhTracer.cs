using MathLibrary.BVH;

namespace MathLibrary.Tracing;

public static class SmoothBvhTracer
{
    public static bool Intersect(SmoothBvhNode? root, Ray ray, out HitInfo hitInfo)
    {
        hitInfo = default;
        if (root == null) return false;

        float closestDistance = float.MaxValue;
        bool hasHit = false;
        var stack = new Stack<SmoothBvhNode>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            // Пропускаем узлы, которые находятся дальше, чем уже найденное пересечение
            // if (!ray.TryIntersect(node.BoundingBox, out float boxDist) || boxDist >= closestDistance) continue;

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
                // Ваша логика обхода внутренних узлов (здесь простая заглушка)
                stack.Push(node.RightChild!);
                stack.Push(node.LeftChild!);
            }
        }
        return hasHit;
    }
}
