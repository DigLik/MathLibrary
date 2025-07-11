namespace MathLibrary.Geometry;

public struct CpuMaterial
{
    // Основные PBR параметры
    public Vector3 BaseColor;      // Базовый цвет (альбедо для неметаллов, цвет отражения для металлов)
    public float Metallic;         // 0.0 для неметаллов (dielectrics), 1.0 для металлов (metals)
    public float Roughness;        // Шероховатость микроповерхности (0.0 = гладко, 1.0 = шероховато)

    // Параметры прозрачности/преломления. Используется для стекла и SSS.
    public float Transmission;     // Пропускание света (0.0 = непрозрачный, 1.0 = полностью прозрачный)
    public float IOR;              // Index of Refraction (индекс преломления)

    // Анизотропия
    public float Anisotropic;      // Степень анизотропии (0.0 = изотропный, 1.0 = сильно вытянутые отражения)

    // Прозрачный лак (Clear Coat)
    public float Clearcoat;        // Интенсивность слоя лака (0.0 = нет, 1.0 = есть)
    public float ClearcoatRoughness; // Шероховатость слоя лака

    // Свечение
    public Vector3 EmissionColor;
    public float EmissionStrength;
};

public interface ISceneObject
{
    Box GetBoundingBox();
    CpuMaterial GetMaterial();
    object GetObjectData();
}
