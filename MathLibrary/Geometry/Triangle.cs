namespace MathLibrary.Geometry;

public class Triangle : ISceneObject
{
    public readonly Vector3 V0, V1, V2;
    private readonly CpuMaterial _material;

    public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, CpuMaterial material)
    {
        V0 = v0;
        V1 = v1;
        V2 = v2;
        _material = material;
    }

    public Box GetBoundingBox()
    {
        Vector3 min = Vector3.Min(V0, Vector3.Min(V1, V2));
        Vector3 max = Vector3.Max(V0, Vector3.Max(V1, V2));
        Vector3 epsilon = new Vector3(Constants.Epsilon, Constants.Epsilon, Constants.Epsilon);
        return new Box(min - epsilon, max + epsilon);
    }

    public CpuMaterial GetMaterial() => _material;
    public object GetObjectData() => this;
}