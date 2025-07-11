using ComputeSharp;

namespace TestProject;

[ThreadGroupSize(DefaultThreadGroupSizes.XY)]
[GeneratedComputeShaderDescriptor]
public readonly partial struct FinalizeShader(
    ReadWriteTexture2D<float4> accumulationBuffer,
    ReadWriteTexture2D<float4> outputTexture,
    uint totalSamplesPerPixel) : IComputeShader
{
    public void Execute()
    {
        int2 pixelCoords = ThreadIds.XY;

        // 1. Читаем накопленный цвет
        float4 accumulatedColor = accumulationBuffer[pixelCoords];

        // 2. Нормализуем
        float scale = 1.0f / totalSamplesPerPixel;
        float3 finalColor = new float3(
            accumulatedColor.X * scale,
            accumulatedColor.Y * scale,
            accumulatedColor.Z * scale);

        // 3. Применяем гамма-коррекцию
        finalColor.X = Hlsl.Pow(Hlsl.Clamp(finalColor.X, 0.0f, 1.0f), 1.0f / 2.2f);
        finalColor.Y = Hlsl.Pow(Hlsl.Clamp(finalColor.Y, 0.0f, 1.0f), 1.0f / 2.2f);
        finalColor.Z = Hlsl.Pow(Hlsl.Clamp(finalColor.Z, 0.0f, 1.0f), 1.0f / 2.2f);

        // 4. Записываем в финальную текстуру
        outputTexture[pixelCoords] = new float4(finalColor.X, finalColor.Y, finalColor.Z, 1.0f);
    }
}
