using MathLibrary.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary.BVH;
public class BvhNode
{
    /// <summary>
    /// Ограничивающий объем, охватывающий все примитивы в этом узле и его дочерних узлах.
    /// </summary>
    public Box BoundingBox { get; }

    /// <summary>
    /// Левый дочерний узел. Null для листовых узлов.
    /// </summary>
    public BvhNode? LeftChild { get; }

    /// <summary>
    /// Правый дочерний узел. Null для листовых узлов.
    /// </summary>
    public BvhNode? RightChild { get; }

    /// <summary>
    /// Список треугольников. Заполнен только для листовых узлов.
    /// </summary>
    public List<Triangle>? Primitives { get; }

    /// <summary>
    /// Является ли этот узел листовым.
    /// </summary>
    public bool IsLeaf => LeftChild == null && RightChild == null;

    // Конструктор для внутреннего узла
    public BvhNode(Box box, BvhNode left, BvhNode right)
    {
        BoundingBox = box;
        LeftChild = left;
        RightChild = right;
        Primitives = null;
    }

    // Конструктор для листового узла
    public BvhNode(Box box, List<Triangle> primitives)
    {
        BoundingBox = box;
        LeftChild = null;
        RightChild = null;
        Primitives = primitives;
    }
}
