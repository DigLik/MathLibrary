namespace MathLibrary.Tracing;

public readonly partial record struct Ray
{
    /// <summary>
    /// Размер луча в байтах.
    /// </summary>
    public const int SizeInBytes = 2 * Vector3.SizeInBytes;

    /// <summary>
    /// Нулевой луч, начинающийся в начале координат и направленный в нулевую точку (0, 0, 0).
    /// </summary>
    public static Ray Zero => new(Vector3.Zero, Vector3.Zero);

    /// <summary>
    /// Единичный луч, направленный вверх (0, 1, 0).
    /// </summary>
    public static Ray Up => new(Vector3.Zero, Vector3.UnitY);
    public static Ray UnitY => Up;

    /// <summary>
    /// Единичный луч, направленный вниз (0, -1, 0).
    /// </summary>
    public static Ray Down => new(Vector3.Zero, -Vector3.UnitY);
    public static Ray UnitYNeg => Down;

    /// <summary>
    /// Единичный луч, направленный влево (-1, 0, 0).
    /// </summary>
    public static Ray Left => new(Vector3.Zero, -Vector3.UnitX);
    public static Ray UnitXNeg => Left;

    /// <summary>
    /// Единичный луч, направленный вправо (1, 0, 0).
    /// </summary>
    public static Ray Right => new(Vector3.Zero, Vector3.UnitX);
    public static Ray UnitX => Right;

    /// <summary>
    /// Единичный луч, направленный вперед (0, 0, 1).
    /// </summary>
    public static Ray Forward => new(Vector3.Zero, Vector3.UnitZ);
    public static Ray UnitZ => Forward;

    /// <summary>
    /// Единичный луч, направленный назад (0, 0, -1).
    /// </summary>
    public static Ray Backward => new(Vector3.Zero, -Vector3.UnitZ);
    public static Ray UnitZNeg => Backward;

    /// <summary>
    /// Луч, представляющий положительную бесконечность, начинающийся в начале координат и направленный в положительную бесконечность.
    /// </summary>
    public static Ray PositiveInfinity => new(Vector3.Zero, Vector3.PositiveInfinity);

    /// <summary>
    /// Луч, представляющий отрицательную бесконечность, начинающийся в начале координат и направленный в отрицательную бесконечность.
    /// </summary>
    public static Ray NegativeInfinity => new(Vector3.Zero, Vector3.NegativeInfinity);
}
