using MathLibrary.Geometry;
using MathLibrary.Tracing;

namespace MathLibrary.BVH;

/// <summary>
/// Строит BVH-дерево из MeshTriangle, используя ReadOnlySpan<T> для производительности.
/// </summary>
public static class SmoothBvhBuilder
{
    public static SmoothBvhNode? Build(ReadOnlySpan<MeshTriangle> triangles, int maxPrimitivesPerNode = 8)
    {
        if (triangles.IsEmpty) return null;

        var nodeBox = CalculateBounds(triangles);

        if (triangles.Length <= maxPrimitivesPerNode)
        {
            // Для листового узла создаем список из спана
            return new SmoothBvhNode(nodeBox, [.. triangles]);
        }

        var extent = nodeBox.Max - nodeBox.Min;
        int axis = 0;
        if (extent.Y > extent.X) axis = 1;
        if (extent.Z > extent[axis]) axis = 2;

        // Создаем массив для сортировки, чтобы не модифицировать исходные данные
        var sortedTriangles = triangles.ToArray();

        // Сортируем по центроиду вдоль самой длинной оси
        Array.Sort(sortedTriangles, (a, b) =>
            GetCentroid(a.Geometry)[axis].CompareTo(GetCentroid(b.Geometry)[axis]));

        int mid = sortedTriangles.Length / 2;

        // Обработка крайнего случая, если разделение не удалось
        // (Все треугольники могут иметь одинаковый центроид по оси)
        if (GetCentroid(sortedTriangles[0].Geometry)[axis] == GetCentroid(sortedTriangles[^1].Geometry)[axis])
        {
            // Просто делим пополам без гарантии пространственного разделения
        }

        var leftSpan = sortedTriangles.AsSpan(0, mid);
        var rightSpan = sortedTriangles.AsSpan(mid);

        var leftChild = Build(leftSpan, maxPrimitivesPerNode);
        var rightChild = Build(rightSpan, maxPrimitivesPerNode);

        // Дочерние узлы не могут быть null, так как мы проверяем на пустоту в начале
        return new SmoothBvhNode(nodeBox, leftChild!, rightChild!);
    }

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