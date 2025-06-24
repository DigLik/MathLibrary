namespace MathLibrary;

public readonly partial record struct Vector2
{
    /// <summary>
    /// Размер структуры в байтах.
    /// </summary>
    public const int SizeInBytes = 2 * sizeof(float);

    /// <summary>
    /// Нулевой вектор (0, 0).
    /// </summary>
    public static Vector2 Zero { get; } = new(0f, 0f);

    /// <summary>
    /// Вектор с компонентами (1, 1).
    /// </summary>
    public static Vector2 One { get; } = new(1f, 1f);

    /// <summary>
    /// Единичный вектор, направленный вверх (0, 1).
    /// </summary>
    public static Vector2 Up { get; } = new(0f, 1f);
    public static Vector2 UnitY => Up;

    /// <summary>
    /// Единичный вектор, направленный вниз (0, -1).
    /// </summary>
    public static Vector2 Down { get; } = new(0f, -1f);
    public static Vector2 UnitYNeg => Down;

    /// <summary>
    /// Единичный вектор, направленный влево (-1, 0).
    /// </summary>
    public static Vector2 Left { get; } = new(-1f, 0f);
    public static Vector2 UnitXNeg => Left;

    /// <summary>
    /// Единичный вектор, направленный вправо (1, 0).
    /// </summary>
    public static Vector2 Right { get; } = new(1f, 0f);
    public static Vector2 UnitX => Right;

    /// <summary>
    /// Вектор, представляющий положительную бесконечность.
    /// </summary>
    public static Vector2 PositiveInfinity { get; } = new(float.PositiveInfinity, float.PositiveInfinity);

    /// <summary>
    /// Вектор, представляющий отрицательную бесконечность.
    /// </summary>
    public static Vector2 NegativeInfinity { get; } = new(float.NegativeInfinity, float.NegativeInfinity);
}
