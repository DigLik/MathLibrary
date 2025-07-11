using ComputeSharp;

namespace TestProject;

[ThreadGroupSize(DefaultThreadGroupSizes.XY)]
[GeneratedComputeShaderDescriptor]
public readonly partial struct ClearShader(ReadWriteTexture2D<float4> buffer) : IComputeShader
{
    public void Execute()
    {
        buffer[ThreadIds.XY] = new float4(0, 0, 0, 0);
    }
}
