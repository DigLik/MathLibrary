namespace MathLibrary.Geometry;

public readonly partial record struct Box
{
    public static Box Combine(Box a, Box b)
    {
        Vector3 min = Vector3.Min(a.Min, b.Min);
        Vector3 max = Vector3.Max(a.Max, b.Max);
        return new Box(min, max);
    }
}
