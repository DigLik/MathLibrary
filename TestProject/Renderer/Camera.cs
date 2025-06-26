using MathLibrary;
using MathLibrary.Tracing;

namespace TestProject.Renderer;

public class Camera
{
    private readonly Vector3 _origin;
    private readonly Vector3 _lowerLeftCorner;
    private readonly Vector3 _horizontal;
    private readonly Vector3 _vertical;

    public Camera(Vector3 lookFrom, Vector3 lookAt, Vector3 viewUp, float verticalFov, float aspectRatio)
    {
        var theta = verticalFov * MathF.PI / 180.0f;
        var h = MathF.Tan(theta / 2.0f);
        var viewportHeight = 2.0f * h;
        var viewportWidth = aspectRatio * viewportHeight;

        var w = Vector3.Normalize(lookFrom - lookAt);
        var u = Vector3.Normalize(Vector3.Cross(viewUp, w));
        var v = Vector3.Cross(w, u);

        _origin = lookFrom;
        _horizontal = viewportWidth * u;
        _vertical = viewportHeight * v;
        _lowerLeftCorner = _origin - _horizontal / 2 - _vertical / 2 - w;
    }

    public Ray GetRay(float u, float v)
    {
        var direction = _lowerLeftCorner + u * _horizontal + v * _vertical - _origin;
        return new Ray(_origin, Vector3.Normalize(direction));
    }
}

