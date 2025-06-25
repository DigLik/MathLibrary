namespace MathLibrary.Geometry;

public readonly partial record struct Box
{
    /// <summary>
    /// Возвращает вектор, представляющий размер (ширину, высоту и глубину) параллелепипеда.
    /// </summary>
    public Vector3 Size => Max - Min;

    /// <summary>
    /// Возвращает центр параллелепипеда.
    /// </summary>
    public Vector3 Center => (Min + Max) * 0.5f;

    /// <summary>
    /// Возвращает объем параллелепипеда.
    /// </summary>
    public float Volume => Size.X * Size.Y * Size.Z;
}
