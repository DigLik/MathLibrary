using MathLibrary.Core;

namespace MathLibrary;

public readonly partial record struct Matrix4x4
{
    /// <summary>
    /// Создает матрицу трансляции (перемещения).
    /// </summary>
    /// <param name="position">Вектор перемещения.</param>
    public static Matrix4x4 CreateTranslation(in Vector3 position) => new(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        position.X, position.Y, position.Z, 1);

    /// <summary>
    /// Создает матрицу масштабирования.
    /// </summary>
    /// <param name="scales">Вектор масштабирования по осям.</param>
    public static Matrix4x4 CreateScale(in Vector3 scales) => new(
        scales.X, 0, 0, 0,
        0, scales.Y, 0, 0,
        0, 0, scales.Z, 0,
        0, 0, 0, 1);

    /// <summary>
    /// Создает матрицу равномерного масштабирования.
    /// </summary>
    /// <param name="scale">Коэффициент масштабирования.</param>
    public static Matrix4x4 CreateScale(float scale) => new(
        scale, 0, 0, 0,
        0, scale, 0, 0,
        0, 0, scale, 0,
        0, 0, 0, 1);

    /// <summary>
    /// Создает матрицу вращения из кватерниона.
    /// </summary>
    /// <param name="q">Кватернион вращения.</param>
    public static Matrix4x4 CreateFromQuaternion(in Quaternion q)
    {
        var xx = q.X * q.X; var yy = q.Y * q.Y; var zz = q.Z * q.Z;
        var xy = q.X * q.Y; var xz = q.X * q.Z; var yz = q.Y * q.Z;
        var wx = q.W * q.X; var wy = q.W * q.Y; var wz = q.W * q.Z;

        return new Matrix4x4(
            1 - 2 * (yy + zz), 2 * (xy + wz), 2 * (xz - wy), 0,
            2 * (xy - wz), 1 - 2 * (xx + zz), 2 * (yz + wx), 0,
            2 * (xz + wy), 2 * (yz - wx), 1 - 2 * (xx + yy), 0,
            0, 0, 0, 1);
    }

    /// <summary>
    /// Создает матрицу вращения на основе углов Эйлера.
    /// </summary>
    /// <param name="eulerAngles">Вектор, содержащий углы тангажа (X), рыскания (Y) и крена (Z) в градусах.</param>
    public static Matrix4x4 CreateFromEulerAngles(in Vector3 eulerAngles)
        => CreateFromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);

    /// <summary>
    /// Создает матрицу вращения на основе углов Эйлера.
    /// </summary>
    /// <param name="pitch">Угол тангажа (вокруг оси X) в градусах.</param>
    /// <param name="yaw">Угол рыскания (вокруг оси Y) в градусах.</param>
    /// <param name="roll">Угол крена (вокруг оси Z) в градусах.</param>
    public static Matrix4x4 CreateFromEulerAngles(float pitch, float yaw, float roll)
    {
        (var sinP, var cosP) = MathF.SinCos(pitch * MathHelper.Deg2Rad);
        (var sinY, var cosY) = MathF.SinCos(yaw * MathHelper.Deg2Rad);
        (var sinR, var cosR) = MathF.SinCos(roll * MathHelper.Deg2Rad);

        // Порядок Z-X-Y: M = My * Mx * Mz
        return new Matrix4x4(
            cosY * cosR + sinY * sinP * sinR, cosR * sinY * sinP - cosY * sinR, cosP * sinY, 0,
            cosP * sinR, cosP * cosR, -sinP, 0,
            cosY * sinP * sinR - sinY * cosR, cosY * sinP * cosR + sinY * sinR, cosP * cosY, 0,
            0, 0, 0, 1
        );
    }

    /// <summary>
    /// Создает матрицу вида (View Matrix) для левосторонней системы координат.
    /// </summary>
    /// <param name="cameraPosition">Позиция камеры.</param>
    /// <param name="cameraTarget">Точка, на которую смотрит камера.</param>
    /// <param name="cameraUpVector">Вектор, определяющий "верх" для камеры.</param>
    public static Matrix4x4 CreateLookAt(in Vector3 cameraPosition, in Vector3 cameraTarget, in Vector3 cameraUpVector)
    {
        var zAxis = (cameraTarget - cameraPosition).Normalize();
        var xAxis = Vector3.Cross(cameraUpVector, zAxis).Normalize();
        var yAxis = Vector3.Cross(zAxis, xAxis);

        return new Matrix4x4(
            xAxis.X, yAxis.X, zAxis.X, 0,
            xAxis.Y, yAxis.Y, zAxis.Y, 0,
            xAxis.Z, yAxis.Z, zAxis.Z, 0,
            -Vector3.Dot(xAxis, cameraPosition), -Vector3.Dot(yAxis, cameraPosition), -Vector3.Dot(zAxis, cameraPosition), 1);
    }

    /// <summary>
    /// Создает матрицу перспективной проекции для левосторонней системы координат.
    /// </summary>
    /// <param name="fieldOfView">Угол обзора по вертикали в радианах.</param>
    /// <param name="aspectRatio">Соотношение сторон (ширина / высота).</param>
    /// <param name="nearPlane">Расстояние до ближней плоскости отсечения.</param>
    /// <param name="farPlane">Расстояние до дальней плоскости отсечения.</param>
    public static Matrix4x4 CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
    {
        var yScale = 1.0f / MathF.Tan(fieldOfView * 0.5f);
        var xScale = yScale / aspectRatio;
        var zRange = farPlane / (farPlane - nearPlane); // Для левосторонней системы

        return new Matrix4x4(
            xScale, 0, 0, 0,
            0, yScale, 0, 0,
            0, 0, zRange, 1, // Для левосторонней системы
            0, 0, -nearPlane * zRange, 0);
    }

    /// <summary>
    /// Создает центрированную ортографическую матрицу проекции.
    /// </summary>
    public static Matrix4x4 CreateOrthographic(float width, float height, float nearPlane, float farPlane)
        => CreateOrthographicOffCenter(-width * 0.5f, width * 0.5f, -height * 0.5f, height * 0.5f, nearPlane, farPlane);

    /// <summary>
    /// Создает ортографическую матрицу проекции со смещением.
    /// </summary>
    public static Matrix4x4 CreateOrthographicOffCenter(float left, float right, float bottom, float top, float nearPlane, float farPlane)
    {
        var rangeX = right - left;
        var rangeY = top - bottom;
        var rangeZ = farPlane - nearPlane;

        if (MathHelper.Approximately(rangeX, 0f) ||
            MathHelper.Approximately(rangeY, 0f) ||
            MathHelper.Approximately(rangeZ, 0f))
        {
            return Identity;
        }

        var m11 = 2.0f / rangeX;
        var m22 = 2.0f / rangeY;
        var m33 = 1.0f / rangeZ; // Для левосторонней системы (0 to 1)
        var m41 = -(right + left) / rangeX;
        var m42 = -(top + bottom) / rangeY;
        var m43 = -nearPlane / rangeZ;

        return new Matrix4x4(
            m11, 0, 0, 0,
            0, m22, 0, 0,
            0, 0, m33, 0,
            m41, m42, m43, 1);
    }
}
