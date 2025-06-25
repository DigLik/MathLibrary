namespace MathLibrary.Geometry;

public readonly partial record struct Box
{
    /// <summary>
    /// Размер структуры в байтах.
    /// </summary>
    public const int SizeInBytes = 2 * Vector3.SizeInBytes; // 2 * (3 * sizeof(float))
}
