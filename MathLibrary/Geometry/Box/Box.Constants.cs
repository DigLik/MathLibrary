namespace MathLibrary.Geometry;

public readonly partial record struct Box
{
    /// <summary>
    /// Размер структуры в байтах.
    /// </summary>
    public const int SizeInBytes = 2 * Vector3.SizeInBytes; // 2 * (3 * sizeof(float))

    /// <summary>
    /// Представляет нулевой параллелепипед, где минимальная и максимальная точки совпадают в начале координат.
    /// </summary>
    public static Box Zero => new(Vector3.Zero, Vector3.Zero);

    /// <summary>
    /// Представляет параллелепипед с минимальной точкой в начале координат и максимальной точкой в (1, 1, 1).
    /// </summary>
    public static Box One => new(Vector3.Zero, Vector3.One);

    /// <summary>
    /// Представляет параллелепипед, который охватывает всё трехмерное пространство.
    /// </summary>
    public static Box Infinity => new(Vector3.NegativeInfinity, Vector3.PositiveInfinity);
}
