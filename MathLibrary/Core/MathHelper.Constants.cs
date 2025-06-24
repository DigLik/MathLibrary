namespace MathLibrary.Core;

public static partial class MathHelper
{
    /// <summary>
    /// Значение Epsilon, используемое для сравнения с фиксированной погрешностью.
    /// </summary>
    public const float Epsilon = 1e-6f;

    /// <summary>
    /// Значение относительной погрешности для соответствующего метода сравнения.
    /// </summary>
    public const float RelativeTolerance = 1e-5f;

    /// <summary>
    /// Значение абсолютной погрешности для соответствующего метода сравнения.
    /// </summary>
    public const float AbsoluteTolerance = 1e-8f;

    /// <summary>
    /// Максимально допустимое количество "единиц в последнем разряде" (ULP) для сравнения.
    /// </summary>
    public const int MaxUlp = 4;

    /// <summary>
    /// Значение числа Пи.
    /// </summary>
    public const float Pi = MathF.PI;

    /// <summary>
    /// Множитель для преобразования радиан в угол в 90 градусов (Pi/2).
    /// </summary>
    public const float PiOver2 = Pi / 2.0f;

    /// <summary>
    /// Множитель для преобразования градусов в радианы.
    /// </summary>
    public const float Deg2Rad = Pi / 180.0f;

    /// <summary>
    /// Множитель для преобразования радиан в градусы.
    /// </summary>
    public const float Rad2Deg = 180.0f / Pi;
}
