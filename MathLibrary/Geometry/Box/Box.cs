using System.Runtime.InteropServices;

namespace MathLibrary.Geometry;

/// <summary>
/// Представляет трехмерный прямоугольный параллелепипед (Box) с минимальной и максимальной точками.
/// </summary>
/// <param name="Min"></param>
/// <param name="Max"></param>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Box(Vector3 Min, Vector3 Max);
