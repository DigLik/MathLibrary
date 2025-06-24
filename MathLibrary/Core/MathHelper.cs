using System.Runtime.CompilerServices;

namespace MathLibrary.Core;

/// <summary>
/// Статический класс, предоставляющий вспомогательные математические функции и константы,
/// включая настраиваемые методы для сравнения чисел с плавающей запятой.
/// </summary>
public static partial class MathHelper
{
    /// <summary>
    /// Получает или задает метод по умолчанию для сравнения чисел с плавающей запятой.
    /// По умолчанию используется <see cref="ComparisonMethod.FixedEpsilon"/>.
    /// </summary>
    public static ComparisonMethod DefaultComparisonMethod { get; set; } = ComparisonMethod.FixedEpsilon;

    /// <summary>
    /// Сравнивает два числа с плавающей запятой на приблизительное равенство, используя метод,
    /// заданный в свойстве <see cref="DefaultComparisonMethod"/>.
    /// </summary>
    /// <param name="a">Первое число.</param>
    /// <param name="b">Второе число.</param>
    /// <returns>True, если числа приблизительно равны, иначе false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Approximately(float a, float b)
    {
        return DefaultComparisonMethod switch
        {
            ComparisonMethod.Direct => CompareDirectly(a, b),
            ComparisonMethod.FixedEpsilon => CompareUsingFixedEpsilon(a, b),
            ComparisonMethod.RelativeEpsilon => CompareUsingRelativeEpsilon(a, b),
            ComparisonMethod.CombinedTolerance => CompareUsingCombinedTolerance(a, b),
            ComparisonMethod.Ulp => CompareUsingUlp(a, b),
            _ => throw new ArgumentOutOfRangeException(DefaultComparisonMethod.ToString(), DefaultComparisonMethod, "Неизвестный метод сравнения.")
        };
    }

    /// <summary>
    /// Метод 1: Прямое сравнение.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CompareDirectly(float a, float b) => a == b;

    /// <summary>
    /// Метод 2: Сравнение с фиксированным эпсилон.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CompareUsingFixedEpsilon(float a, float b) => MathF.Abs(a - b) < Epsilon;

    /// <summary>
    /// Метод 3: Сравнение с относительной погрешностью.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CompareUsingRelativeEpsilon(float a, float b)
    {
        // Обработка случая, когда оба числа очень близки к нулю
        return CompareUsingFixedEpsilon(a, b) || MathF.Abs(a - b) <= RelativeTolerance * MathF.Max(MathF.Abs(a), MathF.Abs(b));
    }

    /// <summary>
    /// Метод 4: Сравнение с комбинированной погрешностью.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CompareUsingCombinedTolerance(float a, float b)
    {
        return MathF.Abs(a - b) <= MathF.Max(AbsoluteTolerance, RelativeTolerance * MathF.Max(MathF.Abs(a), MathF.Abs(b)));
    }

    /// <summary>
    /// Метод 5: Сравнение с использованием ULP.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CompareUsingUlp(float a, float b)
    {
        // Этот метод работает для NaN, бесконечностей и денормализованных чисел.
        var uA = new FloatIntUnion { F = a };
        var uB = new FloatIntUnion { F = b };

        // Разные знаки означают, что числа не равны, если только они не близки к нулю.
        if (uA.I >> 31 != uB.I >> 31)
            // Проверяем, являются ли оба числа нулями (или очень близки к ним)
            return (uA.I & 0x7FFFFFFF) < MaxUlp && (uB.I & 0x7FFFFFFF) < MaxUlp;

        // Числа одного знака, находим разницу в их целочисленных представлениях.
        return Math.Abs(uA.I - uB.I) <= MaxUlp;
    }
}
