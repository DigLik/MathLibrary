using MathLibrary.Geometry;
using MathLibrary.Tracing;

namespace MathLibrary.BVH;
public static class BvhBuilder
{
    /// <summary>
    /// Строит BVH-дерево для списка треугольников.
    /// </summary>
    /// <param name="triangles">Список всех треугольников в сцене.</param>
    /// <param name="maxPrimitivesPerNode">Максимальное количество треугольников в листовом узле.</param>
    /// <returns>Корневой узел построенного BVH-дерева.</returns>
    public static BvhNode? Build(IReadOnlyList<MeshTriangle> meshTriangles, int maxPrimitivesPerNode = 4)
    {
        if (meshTriangles == null || meshTriangles.Count == 0) return null;

        // Создаем рабочий массив один раз, чтобы избежать аллокаций в рекурсии
        var primitives = meshTriangles as MeshTriangle[] ?? [.. meshTriangles];

        return BuildRecursive(primitives.AsSpan(), maxPrimitivesPerNode);
    }

    private static BvhNode BuildRecursive(Span<MeshTriangle> triangles, int maxPrimitivesPerNode)
    {
        var nodeBox = CalculateBounds(triangles);

        // Базовый случай: если примитивов мало, создаем лист
        if (triangles.Length <= maxPrimitivesPerNode)
        {
            return new BvhNode(nodeBox, [.. triangles]);
        }

        // Находим самую длинную ось для разделения
        var extent = nodeBox.Max - nodeBox.Min;
        int axis = 0;
        if (extent.Y > extent.X) axis = 1;
        if (extent.Z > extent[axis]) axis = 2;

        // --- ГЛАВНОЕ ИЗМЕНЕНИЕ: Сортируем срез на месте ---
        // Это гарантирует, что мы разделим количество объектов 50/50,
        // создавая идеально сбалансированное дерево.
        triangles.Sort((a, b) =>
            GetCentroid(a.Geometry)[axis].CompareTo(GetCentroid(b.Geometry)[axis]));

        int mid = triangles.Length / 2;

        // Рекурсивно строим для левой и правой половин, не создавая новых списков
        var leftChild = BuildRecursive(triangles[..mid], maxPrimitivesPerNode);
        var rightChild = BuildRecursive(triangles[mid..], maxPrimitivesPerNode);

        return new BvhNode(nodeBox, leftChild!, rightChild!);
    }

    /// <summary>
    /// Вычисляет AABB, который охватывает список треугольников.
    /// </summary>
    private static Box CalculateBounds(ReadOnlySpan<MeshTriangle> triangles)
    {
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);

        foreach (var meshTri in triangles)
        {
            var tri = meshTri.Geometry;
            min = Vector3.Min(min, Vector3.Min(tri.A, Vector3.Min(tri.B, tri.C)));
            max = Vector3.Max(max, Vector3.Max(tri.A, Vector3.Max(tri.B, tri.C)));
        }
        return new Box(min, max);
    }

    private static Vector3 GetCentroid(Triangle t) => (t.A + t.B + t.C) / 3.0f;
}
