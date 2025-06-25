using System.Runtime.InteropServices;

namespace MathLibrary.Tracing;

/// <summary>
/// Представляет луч (Ray) в трехмерном пространстве.
/// </summary>
/// <param name="Origin"></param>
/// <param name="Direction"></param>
[StructLayout(LayoutKind.Sequential)]
public readonly partial record struct Ray(Vector3 Origin, Vector3 Direction)
{
    /// <summary>
    /// Создает луч с началом в начале координат и направлением (1, 0, 0).
    /// </summary>
    public Ray() : this(Vector3.Zero, Vector3.UnitX) { }

    /// <summary>
    /// Создает луч с заданным началом и направлением.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    public Ray(Vector2 origin, Vector2 direction)
        : this(new Vector3(origin.X, origin.Y, 0), new Vector3(direction.X, direction.Y, 0)) { }
}
