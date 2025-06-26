using MathLibrary.BVH;
using MathLibrary.Geometry;

namespace MathLibrary.Tracing;
public static class BvhTracer
{
    public static bool Intersect(BvhNode? root, Ray ray, out HitInfo hitInfo)
    {
        hitInfo = default;
        if (root == null) return false;

        float closestDistance = float.MaxValue;
        bool hasHit = false;
        var stack = new Stack<BvhNode>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            // if (!ray.TryIntersect(node.BoundingBox, out float boxDist) || boxDist >= closestDistance) continue;

            if (node.IsLeaf)
            {
                // Теперь это корректно, т.к. BvhNode хранит List<MeshTriangle>
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
                stack.Push(node.RightChild!);
                stack.Push(node.LeftChild!);
            }
        }
        return hasHit;
    }
}
