using MathLibrary.Geometry;
using MathLibrary.Tracing;

namespace MathLibrary.BVH;

/// <summary>
/// Представляет узел в BVH, оптимизированный для геометрии с нормалями вершин.
/// </summary>
public class SmoothBvhNode
{
    public Box BoundingBox { get; }
    public SmoothBvhNode? LeftChild { get; }
    public SmoothBvhNode? RightChild { get; }
    public List<MeshTriangle>? Primitives { get; } // Хранит MeshTriangle
    public bool IsLeaf => LeftChild == null && RightChild == null;

    // Конструктор для внутреннего узла
    public SmoothBvhNode(Box box, SmoothBvhNode left, SmoothBvhNode right)
    {
        BoundingBox = box;
        LeftChild = left;
        RightChild = right;
        Primitives = null;
    }

    // Конструктор для листового узла
    public SmoothBvhNode(Box box, List<MeshTriangle> primitives)
    {
        BoundingBox = box;
        LeftChild = null;
        RightChild = null;
        Primitives = primitives;
    }
}
