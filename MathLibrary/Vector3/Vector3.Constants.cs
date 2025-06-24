namespace MathLibrary;

public readonly partial record struct Vector3
{
    /// <summary>
    /// Размер структуры в байтах.
    /// </summary>
    public const int SizeInBytes = 3 * sizeof(float);

    /// <summary>
    /// Нулевой вектор (0, 0, 0).
    /// </summary>
    public static Vector3 Zero { get; } = new(0f, 0f, 0f);

    /// <summary>
    /// Вектор с компонентами (1, 1, 1).
    /// </summary>
    public static Vector3 One { get; } = new(1f, 1f, 1f);

    /// <summary>
    /// Единичный вектор, направленный вверх (0, 1, 0).
    /// </summary>
    public static Vector3 Up { get; } = new(0f, 1f, 0f);
    public static Vector3 UnitY => Up;

    /// <summary>
    /// Единичный вектор, направленный вниз (0, -1, 0).
    /// </summary>
    public static Vector3 Down { get; } = new(0f, -1f, 0f);
    public static Vector3 UnitYNeg => Down;

    /// <summary>
    /// Единичный вектор, направленный влево (-1, 0, 0).
    /// </summary>
    public static Vector3 Left { get; } = new(-1f, 0f, 0f);
    public static Vector3 UnitXNeg => Left;

    /// <summary>
    /// Единичный вектор, направленный вправо (1, 0, 0).
    /// </summary>
    public static Vector3 Right { get; } = new(1f, 0f, 0f);
    public static Vector3 UnitX => Right;

    /// <summary>
    /// Единичный вектор, направленный вперед (0, 0, 1).
    /// </summary>
    public static Vector3 Forward { get; } = new(0f, 0f, 1f);
    public static Vector3 UnitZ => Forward;

    /// <summary>
    /// Единичный вектор, направленный назад (0, 0, -1).
    /// </summary>
    public static Vector3 Backward { get; } = new(0f, 0f, -1f);
    public static Vector3 UnitZNeg => Backward;

    /// <summary>
    /// Вектор, представляющий положительную бесконечность.
    /// </summary>
    public static Vector3 PositiveInfinity { get; } = new(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

    /// <summary>
    /// Вектор, представляющий отрицательную бесконечность.
    /// </summary>
    public static Vector3 NegativeInfinity { get; } = new(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
}
