using System.Runtime.InteropServices;

namespace MathLibrary;

/// <summary>
/// Трехмерный вектор.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Vector3(float X, float Y, float Z);
