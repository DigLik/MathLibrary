using MathLibrary.Geometry;

namespace MathLibrary.BVH;
public static class BvhBuilder
{
    /// <summary>
    /// Строит BVH-дерево для списка треугольников.
    /// </summary>
    /// <param name="triangles">Список всех треугольников в сцене.</param>
    /// <param name="maxPrimitivesPerNode">Максимальное количество треугольников в листовом узле.</param>
    /// <returns>Корневой узел построенного BVH-дерева.</returns>
    public static BvhNode Build(List<Triangle> triangles, int maxPrimitivesPerNode = 4)
    {
        return triangles.Count == 0
            ? throw new ArgumentException("Cannot build BVH for an empty list of triangles.")
            : BuildRecursive(triangles, maxPrimitivesPerNode);
    }

    private static BvhNode BuildRecursive(List<Triangle> triangles, int maxPrimitivesPerNode)
    {
        // Вычисляем ограничивающий объем для текущего набора треугольников
        var nodeBox = CalculateBounds(triangles);

        // Базовый случай рекурсии: если треугольников мало, создаем листовой узел
        if (triangles.Count <= maxPrimitivesPerNode)
        {
            return new BvhNode(nodeBox, [.. triangles]);
        }

        // 1. Находим самую длинную ось ограничивающего объема
        Vector3 extent = nodeBox.Max - nodeBox.Min;
        int axis = 0; // 0=X, 1=Y, 2=Z
        if (extent.Y > extent.X) axis = 1;
        if (extent.Z > extent.Y && extent.Z > extent.X) axis = 2;

        // 2. Определяем точку разделения (медиана по выбранной оси)
        float splitPos = nodeBox.Min[axis] + extent[axis] * 0.5f;

        // 3. Разделяем треугольники на два списка на основе положения их центроидов
        var leftPrimitives = new List<Triangle>();
        var rightPrimitives = new List<Triangle>();

        foreach (var tri in triangles)
        {
            Vector3 centroid = (tri.A + tri.B + tri.C) / 3.0f;
            if (centroid[axis] < splitPos)
            {
                leftPrimitives.Add(tri);
            }
            else
            {
                rightPrimitives.Add(tri);
            }
        }

        // Обработка крайнего случая: если все треугольники попали в один из списков
        if (leftPrimitives.Count == 0 || rightPrimitives.Count == 0)
        {
            // Просто делим исходный список пополам, чтобы избежать бесконечной рекурсии
            int mid = triangles.Count / 2;
            leftPrimitives = triangles.GetRange(0, mid);
            rightPrimitives = triangles.GetRange(mid, triangles.Count - mid);
        }

        // 4. Рекурсивно строим дочерние узлы
        var leftChild = BuildRecursive(leftPrimitives, maxPrimitivesPerNode);
        var rightChild = BuildRecursive(rightPrimitives, maxPrimitivesPerNode);

        return new BvhNode(nodeBox, leftChild, rightChild);
    }

    /// <summary>
    /// Вычисляет AABB, который охватывает список треугольников.
    /// </summary>
    private static Box CalculateBounds(List<Triangle> triangles)
    {
        if (triangles.Count == 0)
        {
            return new Box(Vector3.Zero, Vector3.Zero);
        }

        Vector3 min = float.MaxValue;
        Vector3 max = float.MinValue;

        foreach (var tri in triangles)
        {
            min = Vector3.Min(min, tri.A);
            min = Vector3.Min(min, tri.B);
            min = Vector3.Min(min, tri.C);

            max = Vector3.Max(max, tri.A);
            max = Vector3.Max(max, tri.B);
            max = Vector3.Max(max, tri.C);
        }
        return new Box(min, max);
    }
}
