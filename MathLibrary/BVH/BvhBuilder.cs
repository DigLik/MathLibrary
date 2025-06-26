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
    public static BvhNode? Build(IReadOnlyList<MeshTriangle> meshTriangles, int maxPrimitivesPerNode = 8)
    {
        return meshTriangles == null || meshTriangles.Count == 0
            ? null
            : BuildRecursive([.. meshTriangles], maxPrimitivesPerNode);
    }

    private static BvhNode BuildRecursive(List<MeshTriangle> meshTriangles, int maxPrimitivesPerNode)
    {
        var nodeBox = CalculateBounds(meshTriangles);

        if (meshTriangles.Count <= maxPrimitivesPerNode)
        {
            return new BvhNode(nodeBox, meshTriangles);
        }

        var extent = nodeBox.Max - nodeBox.Min;
        int axis = 0;
        if (extent.Y > extent.X) axis = 1;
        if (extent.Z > extent[axis]) axis = 2;

        float splitPos = nodeBox.Min[axis] + extent[axis] * 0.5f;

        var leftPrimitives = new List<MeshTriangle>();
        var rightPrimitives = new List<MeshTriangle>();

        foreach (var tri in meshTriangles)
        {
            Vector3 centroid = (tri.Geometry.A + tri.Geometry.B + tri.Geometry.C) / 3.0f;
            if (centroid[axis] < splitPos) leftPrimitives.Add(tri);
            else rightPrimitives.Add(tri);
        }

        if (leftPrimitives.Count == 0 || rightPrimitives.Count == 0)
        {
            int mid = meshTriangles.Count / 2;
            leftPrimitives = meshTriangles.GetRange(0, mid);
            rightPrimitives = meshTriangles.GetRange(mid, meshTriangles.Count - mid);
        }

        var leftChild = BuildRecursive(leftPrimitives, maxPrimitivesPerNode);
        var rightChild = BuildRecursive(rightPrimitives, maxPrimitivesPerNode);

        return new BvhNode(nodeBox, leftChild, rightChild);
    }

    /// <summary>
    /// Вычисляет AABB, который охватывает список треугольников.
    /// </summary>
    private static Box CalculateBounds(List<MeshTriangle> meshTriangles)
    {
        if (meshTriangles.Count == 0) return new Box(Vector3.Zero, Vector3.Zero);

        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);

        foreach (var meshTri in meshTriangles)
        {
            var tri = meshTri.Geometry;
            min = Vector3.Min(min, Vector3.Min(tri.A, Vector3.Min(tri.B, tri.C)));
            max = Vector3.Max(max, Vector3.Max(tri.A, Vector3.Max(tri.B, tri.C)));
        }
        return new Box(min, max);
    }
}
