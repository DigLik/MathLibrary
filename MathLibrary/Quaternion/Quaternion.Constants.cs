namespace MathLibrary;

public readonly partial record struct Quaternion
{
    /// <summary>
    /// Размер структуры в байтах.
    /// </summary>
    public const int SizeInBytes = 4 * sizeof(float);

    /// <summary>
    /// Кватернион, представляющий нулевое вращение (0, 0, 0, 0).
    /// </summary>
    public static Quaternion Zero { get; } = new(0f, 0f, 0f, 0f);

    /// <summary>
    /// Кватернион, представляющий отсутствие вращения (0, 0, 0, 1).
    /// </summary>
    public static Quaternion Identity { get; } = new(0f, 0f, 0f, 1f);
}
