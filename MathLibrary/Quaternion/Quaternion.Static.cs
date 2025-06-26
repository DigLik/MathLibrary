using MathLibrary.Core;

namespace MathLibrary;

public readonly partial record struct Quaternion
{
    /// <summary>
    /// Создает кватернион, представляющий вращение вокруг указанной оси на заданный угол.
    /// </summary>
    /// <param name="axis">Ось вращения. Должна быть нормализована.</param>
    /// <param name="angle">Угол вращения в радианах.</param>
    public static Quaternion AngleAxis(in Vector3 axis, float angle)
    {
        var halfAngle = angle * 0.5f;
        var s = MathF.Sin(halfAngle);
        return new Quaternion(axis.X * s, axis.Y * s, axis.Z * s, MathF.Cos(halfAngle));
    }

    public static Quaternion CreateFromAxisAngle(in Vector3 axis, float angle)
    {
        return MathHelper.Approximately(axis.MagnitudeSquared, 0f)
            ? Identity
            : AngleAxis(axis.Normalize(), angle);
    }

    /// <summary>
    /// Создает кватернион из углов Эйлера.
    /// </summary>
    /// <param name="eulerAngles">Вектор, содержащий углы тангажа (X), рыскания (Y) и крена (Z) в градусах.</param>
    public static Quaternion CreateFromEulerAngles(in Vector3 eulerAngles)
        => CreateFromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);

    /// <summary>
    /// Создает кватернион из углов Эйлера.
    /// </summary>
    /// <param name="pitch">Угол тангажа (вокруг оси X) в градусах.</param>
    /// <param name="yaw">Угол рыскания (вокруг оси Y) в градусах.</param>
    /// <param name="roll">Угол крена (вокруг оси Z) в градусах.</param>
    /// <remarks>Вращение применяется в порядке: сначала крен (Z), затем тангаж (X), затем рыскание (Y).</remarks>
    public static Quaternion CreateFromEulerAngles(float pitch, float yaw, float roll)
    {
        (var sinP, var cosP) = MathF.SinCos(pitch * MathHelper.Deg2Rad * 0.5f);
        (var sinY, var cosY) = MathF.SinCos(yaw * MathHelper.Deg2Rad * 0.5f);
        (var sinR, var cosR) = MathF.SinCos(roll * MathHelper.Deg2Rad * 0.5f);

        return new Quaternion(
            cosY * sinP * cosR + sinY * cosP * sinR,
            sinY * cosP * cosR - cosY * sinP * sinR,
            cosY * cosP * sinR - sinY * sinP * cosR,
            cosY * cosP * cosR + sinY * sinP * sinR
        );
    }

    /// <summary>
    /// Создает кватернион из матрицы вращения 4x4.
    /// </summary>
    /// <param name="matrix">Матрица вращения. Компоненты трансляции и масштабирования будут проигнорированы.</param>
    public static Quaternion CreateFromRotationMatrix(in Matrix4x4 matrix)
    {
        var trace = matrix.M11 + matrix.M22 + matrix.M33;
        float qx, qy, qz, qw;

        if (trace > 0)
        {
            var s = 0.5f / MathF.Sqrt(trace + 1.0f);
            qw = 0.25f / s;
            qx = (matrix.M32 - matrix.M23) * s;
            qy = (matrix.M13 - matrix.M31) * s;
            qz = (matrix.M21 - matrix.M12) * s;
        }
        else if (matrix.M11 > matrix.M22 && matrix.M11 > matrix.M33)
        {
            var s = 2.0f * MathF.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
            qw = (matrix.M32 - matrix.M23) / s;
            qx = 0.25f * s;
            qy = (matrix.M12 + matrix.M21) / s;
            qz = (matrix.M13 + matrix.M31) / s;
        }
        else if (matrix.M22 > matrix.M33)
        {
            var s = 2.0f * MathF.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
            qw = (matrix.M13 - matrix.M31) / s;
            qx = (matrix.M12 + matrix.M21) / s;
            qy = 0.25f * s;
            qz = (matrix.M23 + matrix.M32) / s;
        }
        else
        {
            var s = 2.0f * MathF.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
            qw = (matrix.M21 - matrix.M12) / s;
            qx = (matrix.M13 + matrix.M31) / s;
            qy = (matrix.M23 + matrix.M32) / s;
            qz = 0.25f * s;
        }
        return new Quaternion(qx, qy, qz, qw);
    }

    /// <summary>
    /// Создает кватернион вращения, который поворачивает из направления `fromDirection` в `toDirection`.
    /// </summary>
    public static Quaternion FromToRotation(in Vector3 fromDirection, in Vector3 toDirection)
    {
        var dot = Vector3.Dot(fromDirection, toDirection);
        if (dot > 0.999999f) return Identity;

        if (dot < -0.999999f)
        {
            var ortho = Vector3.Cross(Vector3.Forward, fromDirection);
            if (ortho.MagnitudeSquared < 0.0001f)
                ortho = Vector3.Cross(Vector3.Up, fromDirection);
            return AngleAxis(ortho.Normalize(), MathHelper.Pi);
        }

        var cross = Vector3.Cross(fromDirection, toDirection);
        var w = MathF.Sqrt(fromDirection.MagnitudeSquared * toDirection.MagnitudeSquared) + dot;
        return new Quaternion(cross.X, cross.Y, cross.Z, w).Normalize();
    }

    /// <summary>
    /// Создает кватернион вращения, который "смотрит" в направлении `forward` с `upwards` в качестве верха.
    /// </summary>
    public static Quaternion LookRotation(in Vector3 forward, in Vector3 upwards)
    {
        var zAxis = forward.Normalize();
        var xAxis = Vector3.Cross(upwards, zAxis).Normalize();
        var yAxis = Vector3.Cross(zAxis, xAxis);

        var matrix = new Matrix4x4(
            xAxis.X, yAxis.X, zAxis.X, 0,
            xAxis.Y, yAxis.Y, zAxis.Y, 0,
            xAxis.Z, yAxis.Z, zAxis.Z, 0,
            0, 0, 0, 1);

        return CreateFromRotationMatrix(in matrix);
    }

    /// <summary>
    /// Вычисляет скалярное произведение двух кватернионов.
    /// </summary>
    public static float Dot(in Quaternion a, in Quaternion b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

    /// <summary>
    /// Линейно интерполирует между двумя кватернионами.
    /// </summary>
    public static Quaternion Lerp(in Quaternion a, in Quaternion b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return (a * (1f - t) + b * t).Normalize();
    }

    /// <summary>
    /// Сферически интерполирует между двумя кватернионами, обеспечивая постоянную скорость вращения.
    /// </summary>
    public static Quaternion Slerp(in Quaternion a, in Quaternion b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        var dot = Dot(a, b);
        var bTemp = b;

        if (dot < 0.0f)
        {
            bTemp = new Quaternion(-b.X, -b.Y, -b.Z, -b.W);
            dot = -dot;
        }

        const float dotThreshold = 0.9995f;
        if (dot > dotThreshold)
            return Lerp(a, bTemp, t);

        var theta_0 = MathF.Acos(dot);
        var theta = theta_0 * t;
        var sin_theta = MathF.Sin(theta);
        var sin_theta_0 = MathF.Sin(theta_0);

        var s0 = MathF.Cos(theta) - dot * sin_theta / sin_theta_0;
        var s1 = sin_theta / sin_theta_0;

        return a * s0 + bTemp * s1;
    }
}
