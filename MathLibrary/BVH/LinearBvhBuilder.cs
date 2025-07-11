using MathLibrary.Geometry;

namespace MathLibrary.BVH;

public class LinearBvhBuilder
{
    private struct PrimitiveInfo
    {
        public int OriginalIndex;
        public Box BoundingBox;
        public Vector3 Centroid;
    }

    private struct BucketInfo
    {
        public int Count;
        public Box Bounds;
    }

    private readonly int _maxPrimitivesPerNode;
    private readonly IReadOnlyList<ISceneObject> _sourceObjects;
    private PrimitiveInfo[] _primitiveInfo;
    private List<CpuBvhNode> _nodes;
    private int _totalNodes;

    public static (CpuBvhNode[] nodes, int[] primitiveIndices) Build(IReadOnlyList<ISceneObject> objects, int maxPrimitivesPerNode = 4)
    {
        var builder = new LinearBvhBuilder(objects, maxPrimitivesPerNode);
        return builder.Run();
    }

    private LinearBvhBuilder(IReadOnlyList<ISceneObject> objects, int maxPrimitivesPerNode)
    {
        _sourceObjects = objects;
        _maxPrimitivesPerNode = Math.Max(1, maxPrimitivesPerNode);
    }

    private (CpuBvhNode[] nodes, int[] primitiveIndices) Run()
    {
        if (_sourceObjects.Count == 0)
        {
            return (Array.Empty<CpuBvhNode>(), Array.Empty<int>());
        }

        _primitiveInfo = new PrimitiveInfo[_sourceObjects.Count];
        for (int i = 0; i < _sourceObjects.Count; i++)
        {
            var bbox = _sourceObjects[i].GetBoundingBox();
            _primitiveInfo[i] = new PrimitiveInfo
            {
                OriginalIndex = i,
                BoundingBox = bbox,
                Centroid = bbox.Center
            };
        }

        _nodes = new List<CpuBvhNode>(_sourceObjects.Count * 2);
        _totalNodes = 0;

        RecursiveBuild(0, _sourceObjects.Count);

        var orderedPrimitiveIndices = new int[_sourceObjects.Count];
        for (int i = 0; i < _sourceObjects.Count; i++)
        {
            orderedPrimitiveIndices[i] = _primitiveInfo[i].OriginalIndex;
        }

        return (_nodes.ToArray(), orderedPrimitiveIndices);
    }

    private int RecursiveBuild(int start, int end)
    {
        int currentNodeIndex = _totalNodes++;
        _nodes.Add(new CpuBvhNode());

        Box totalBounds = Box.Empty;
        for (int i = start; i < end; i++)
        {
            totalBounds = Box.Combine(totalBounds, _primitiveInfo[i].BoundingBox);
        }

        int primitiveCount = end - start;

        if (primitiveCount <= _maxPrimitivesPerNode)
        {
            _nodes[currentNodeIndex] = CreateLeafNode(totalBounds, start, primitiveCount);
            return currentNodeIndex;
        }

        Box centroidBounds = Box.Empty;
        for (int i = start; i < end; i++)
        {
            centroidBounds = Box.Combine(centroidBounds, _primitiveInfo[i].Centroid);
        }

        int splitAxis = centroidBounds.MaxExtentAxis();

        if (centroidBounds.Size[splitAxis] < Constants.Epsilon)
        {
            _nodes[currentNodeIndex] = CreateLeafNode(totalBounds, start, primitiveCount);
            return currentNodeIndex;
        }

        const int numBuckets = 12;
        var buckets = new BucketInfo[numBuckets];
        for (int i = 0; i < numBuckets; ++i)
        {
            buckets[i].Bounds = Box.Empty;
        }

        for (int i = start; i < end; i++)
        {
            int b = (int)(numBuckets * centroidBounds.Offset(_primitiveInfo[i].Centroid, splitAxis));
            b = Math.Clamp(b, 0, numBuckets - 1);
            buckets[b].Count++;
            buckets[b].Bounds = Box.Combine(buckets[b].Bounds, _primitiveInfo[i].BoundingBox);
        }

        var costs = new float[numBuckets - 1];
        float totalSurfaceArea = totalBounds.SurfaceArea();
        if (totalSurfaceArea <= 0) totalSurfaceArea = 1;

        for (int i = 0; i < numBuckets - 1; i++)
        {
            Box b0 = Box.Empty;
            int count0 = 0;
            for (int j = 0; j <= i; j++)
            {
                b0 = Box.Combine(b0, buckets[j].Bounds);
                count0 += buckets[j].Count;
            }

            Box b1 = Box.Empty;
            int count1 = 0;
            for (int j = i + 1; j < numBuckets; j++)
            {
                b1 = Box.Combine(b1, buckets[j].Bounds);
                count1 += buckets[j].Count;
            }

            costs[i] = 0.1f + (count0 * b0.SurfaceArea() + count1 * b1.SurfaceArea()) / totalSurfaceArea;
        }

        float minCost = costs[0];
        int minCostSplitBucket = 0;
        for (int i = 1; i < numBuckets - 1; i++)
        {
            if (costs[i] < minCost)
            {
                minCost = costs[i];
                minCostSplitBucket = i;
            }
        }

        float leafCost = primitiveCount;
        if (primitiveCount > _maxPrimitivesPerNode && minCost < leafCost)
        {
            int mid = Partition(_primitiveInfo, start, end, p =>
            {
                int b = (int)(numBuckets * centroidBounds.Offset(p.Centroid, splitAxis));
                b = Math.Clamp(b, 0, numBuckets - 1);
                return b <= minCostSplitBucket;
            });

            if (mid == start || mid == end)
            {
                mid = start + (end - start) / 2;
            }

            int leftChildIndex = RecursiveBuild(start, mid);
            int rightChildIndex = RecursiveBuild(mid, end);

            _nodes[currentNodeIndex] = new CpuBvhNode
            {
                BoundingBox = totalBounds,
                LeftChildIndex = leftChildIndex,
                RightChildIndex = rightChildIndex,
                PrimitiveCount = 0,
                FirstPrimitiveIndex = 0
            };
        }
        else
        {
            _nodes[currentNodeIndex] = CreateLeafNode(totalBounds, start, primitiveCount);
        }

        return currentNodeIndex;
    }

    private static CpuBvhNode CreateLeafNode(Box bounds, int start, int count)
    {
        return new CpuBvhNode
        {
            BoundingBox = bounds,
            FirstPrimitiveIndex = start,
            PrimitiveCount = count,
        };
    }

    private static int Partition<T>(T[] data, int start, int end, Predicate<T> pred)
    {
        int i = start;
        for (int j = start; j < end; j++)
        {
            if (pred(data[j]))
            {
                (data[i], data[j]) = (data[j], data[i]);
                i++;
            }
        }
        return i;
    }
}