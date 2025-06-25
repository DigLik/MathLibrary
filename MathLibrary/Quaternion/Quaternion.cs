using System.Runtime.InteropServices;

namespace MathLibrary;

/// <summary>
/// Кватернион.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Quaternion(float X, float Y, float Z, float W);
