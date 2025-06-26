using MathLibrary.BVH;
using MathLibrary.Geometry;

namespace MathLibrary.Tracing;
public static class BvhTracer
{
    public static bool Intersect(BvhNode? root, Ray ray, out HitInfo hitInfo)
    {
        hitInfo = default;
        if (root == null)
        {
            return false;
        }

        // Ранний выход, если луч не пересекает даже корневой Bounding Box.
        // Предполагается, что у вас есть метод TryIntersect для Ray и Box.
        // if (!ray.TryIntersect(root.BoundingBox, out _))
        // {
        //     return false;
        // }

        float closestDistance = float.MaxValue;
        bool hasHit = false;

        var stack = new Stack<BvhNode>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            // Пропускаем узлы, которые находятся дальше, чем уже найденное пересечение
            // if (!ray.TryIntersect(node.BoundingBox, out float boxDist) || boxDist >= closestDistance)
            // {
            //     continue;
            // }

            if (node.IsLeaf)
            {
                // Если это лист, проверяем пересечение со всеми его треугольниками.
                foreach (var meshTriangle in node.Primitives!)
                {
                    // Вызываем обновленный метод, который возвращает и u, v
                    if (ray.TryIntersect(meshTriangle, out float distance, out float u, out float v))
                    {
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            // === ЭТО ИСПРАВЛЕНИЕ: Создаем HitInfo со всеми 4 параметрами ===
                            hitInfo = new HitInfo(meshTriangle, distance, u, v);
                            hasHit = true;
                        }
                    }
                }
            }
            else // Внутренний узел
            {
                // Проверяем пересечение с Bounding Box'ами дочерних узлов.
                // bool hitLeft = ray.TryIntersect(node.LeftChild!.BoundingBox, out float distLeft);
                // bool hitRight = ray.TryIntersect(node.RightChild!.BoundingBox, out float distRight);
                bool hitLeft = true, hitRight = true; // Заглушки, используйте ваш код
                float distLeft = 0, distRight = 0;

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

        return hasHit;
    }
}
