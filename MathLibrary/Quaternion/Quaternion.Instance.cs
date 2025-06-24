using MathLibrary.Core;

namespace MathLibrary;

public readonly partial record struct Quaternion
{
    /// <summary>
    /// Возвращает квадрат длины кватерниона.
    /// </summary>
    public float MagnitudeSquared => X * X + Y * Y + Z * Z + W * W;

    /// <summary>
    /// Возвращает длину (величину) кватерниона.
    /// </summary>
    public float Magnitude => MathF.Sqrt(MagnitudeSquared);

    /// <summary>
    /// Возвращает углы Эйлера (в градусах), соответствующие данному вращению.
    /// </summary>
    /// <remarks>
    /// Углы возвращаются в виде Vector3(Pitch, Yaw, Roll).
    /// Порядок вращения: Z (Roll), X (Pitch), Y (Yaw).
    /// Этот метод подвержен явлению "Gimbal Lock".
    /// </remarks>
    public Vector3 EulerAngles
    {
        get
        {
            float pitch, yaw, roll;

            // Тангаж (Pitch, вращение вокруг X)
            var sinP = 2.0f * (W * X + Y * Z);
            pitch = MathF.Abs(sinP) >= 1 ? MathF.CopySign(MathHelper.PiOver2, sinP) : MathF.Asin(sinP);

            // Рыскание (Yaw, вращение вокруг Y)
            var sinYCosP = 2.0f * (W * Y - X * Z);
            var cosYCosP = 1.0f - 2.0f * (X * X + Y * Y);
            yaw = MathF.Atan2(sinYCosP, cosYCosP);

            // Крен (Roll, вращение вокруг Z)
            var sinRCosP = 2.0f * (W * Z + X * Y);
            var cosRCosP = 1.0f - 2.0f * (X * X + Z * Z);
            roll = MathF.Atan2(sinRCosP, cosRCosP);

            return new Vector3(pitch * MathHelper.Rad2Deg, yaw * MathHelper.Rad2Deg, roll * MathHelper.Rad2Deg);
        }
    }

    /// <summary>
    /// Возвращает новый кватернион, являющийся нормализованной версией данного.
    /// </summary>
    public Quaternion Normalize()
    {
        var mag = Magnitude;
        return MathHelper.Approximately(mag, 0f)
            ? throw new DivideByZeroException("Невозможно нормализовать нулевой кватернион.")
            : this / mag;
    }

    /// <summary>
    /// Пытается нормализовать кватернион.
    /// </summary>
    public bool TryNormalize(out Quaternion result)
    {
        var magSq = MagnitudeSquared;
        if (MathHelper.Approximately(magSq, 0f))
        {
            result = Identity;
            return false;
        }
        var mag = MathF.Sqrt(magSq);
        result = new Quaternion(X / mag, Y / mag, Z / mag, W / mag);
        return true;
    }

    /// <summary>
    /// Возвращает инвертированный (сопряженный) кватернион. Для единичных кватернионов это эквивалентно обратному вращению.
    /// </summary>
    public Quaternion Inverse() => new(-X, -Y, -Z, W);

    /// <summary>
    /// Сравнивает данный кватернион с другим на приблизительное равенство.
    /// </summary>
    public bool ApproximatelyEquals(in Quaternion other)
        => MathHelper.Approximately(X, other.X) &&
           MathHelper.Approximately(Y, other.Y) &&
           MathHelper.Approximately(Z, other.Z) &&
           MathHelper.Approximately(W, other.W);

    /// <summary>
    /// Преобразует кватернион в представление "ось-угол".
    /// </summary>
    public void ToAngleAxis(out Vector3 axis, out float angle)
    {
        if (!TryNormalize(out var q))
        {
            axis = Vector3.Forward;
            angle = 0f;
            return;
        }

        angle = 2.0f * MathF.Acos(q.W);
        var s = MathF.Sqrt(1.0f - q.W * q.W);

        axis = MathHelper.Approximately(s, 0f)
            ? Vector3.Forward
            : new Vector3(q.X / s, q.Y / s, q.Z / s);
    }
}
