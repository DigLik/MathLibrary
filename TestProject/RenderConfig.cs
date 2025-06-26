namespace TestProject;

public readonly record struct RenderConfig()
{
    public int ImageWidth { get; init; } = 800;
    public int ImageHeight { get; init; } = 800;
    public int SamplesPerPixel { get; init; } = 256;
    public int MaxDepth { get; init; } = 8;
    public string OutputFileName { get; init; } = "render.png";
}
