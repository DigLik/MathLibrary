using MathLibrary.Geometry;

namespace MathLibrary.BVH;

public struct CpuBvhNode
{
    public Box BoundingBox;
    public int LeftChildIndex;
    public int RightChildIndex;
    public int PrimitiveCount;
    public int FirstPrimitiveIndex;
}
