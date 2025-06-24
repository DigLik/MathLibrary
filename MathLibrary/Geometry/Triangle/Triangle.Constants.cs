namespace MathLibrary.Geometry;

public readonly partial record struct Triangle
{
    /// <summary>
    /// Размер структуры в байтах.
    /// </summary>
    public const int SizeInBytes = 3 * Vector3.SizeInBytes; // 3 * (3 * sizeof(float))
}
