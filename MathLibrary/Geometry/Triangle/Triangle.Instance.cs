namespace MathLibrary.Geometry;

public readonly partial record struct Triangle
{
    /// <summary>
    /// Cвойство для вычисления векторного произведения ребер.
    /// Используется для расчета и нормали, и площади.
    /// </summary>
    public Vector3 CrossProduct => Vector3.Cross(B - A, C - A);

    /// <summary>
    /// Возвращает нормаль треугольника (единичный вектор, перпендикулярный его плоскости).
    /// </summary>
    public Vector3 Normal => CrossProduct.Normalize();

    /// <summary>
    /// Возвращает площадь треугольника.
    /// </summary>
    public float Area => 0.5f * CrossProduct.Magnitude;
}
