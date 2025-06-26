using MathLibrary.BVH;
using MathLibrary.Geometry;

namespace MathLibrary.Tracing;
public static class BvhTracer
{
    public static bool Intersect(BvhNode? root, Ray ray, out HitInfo hitInfo)
    {
        hitInfo = default;
        if (root == null || !ray.TryIntersect(root.BoundingBox, out _))
        {
            return false;
        }

        float closestDistance = float.MaxValue;
        Triangle? closestTriangle = null;

        var stack = new Stack<BvhNode>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (node.IsLeaf)
            {
                foreach (var triangle in node.Primitives!)
                {
                    if (ray.TryIntersect(triangle, out float distance) && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTriangle = triangle;
                    }
                }
            }
            else
            {
                // Проверяем пересечение с дочерними узлами
                bool hitLeft = ray.TryIntersect(node.LeftChild!.BoundingBox, out float distLeft);
                bool hitRight = ray.TryIntersect(node.RightChild!.BoundingBox, out float distRight);

                // Добавляем в стек только те узлы, которые могут содержать более близкое пересечение
                hitLeft &= distLeft < closestDistance;
                hitRight &= distRight < closestDistance;

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

        if (closestTriangle.HasValue)
        {
            hitInfo = new HitInfo(closestTriangle.Value, closestDistance);
            return true;
        }

        return false;
    }
}
