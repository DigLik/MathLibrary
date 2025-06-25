using System.Runtime.InteropServices;

namespace MathLibrary;

/// <summary>
/// Двухмерный вектор.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Vector2(float X, float Y);
