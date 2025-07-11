namespace MathLibrary.Geometry;

public class Sphere(Vector3 center, float radius, CpuMaterial material) : ISceneObject
{
    public readonly Vector3 Center = center;
    public readonly float Radius = radius;
    public readonly CpuMaterial Material = material;

    public Box GetBoundingBox()
    {
        var rVec = new Vector3(Radius, Radius, Radius);
        return new Box(Center - rVec, Center + rVec);
    }

    public CpuMaterial GetMaterial() => Material;
    public object GetObjectData() => this;
}