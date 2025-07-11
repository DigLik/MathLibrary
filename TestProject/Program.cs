using ComputeSharp;
using MathLibrary.BVH;
using MathLibrary.Geometry;
using SixLabors.ImageSharp;
using System.Diagnostics;
using TestProject;
using CpuVector3 = MathLibrary.Geometry.Vector3;
using Rgba32 = SixLabors.ImageSharp.PixelFormats.Rgba32;

// 8K:6, 4K:3, 2K:2, FHD:1.5, HD:1, SD:0.5
const double resolutionMultiplier = 1;
const int width = (int)(1280 * resolutionMultiplier);
const int height = (int)(720 * resolutionMultiplier);
const uint totalSamplesPerPixel = 1024 * 2; // Общее количество сэмплов
const int maxBounces = 128;
const string outputPath = @"C:\Users\12563\Desktop\image.png";

const int tileSize = 256; // Размер плитки
const uint batchSize = 128; // Количество сэмплов за один вызов шейдера

Console.CursorVisible = false;

var red = new CpuMaterial { BaseColor = new(0.65f, 0.05f, 0.05f), Roughness = 0.8f, Metallic = 0.1f };
var green = new CpuMaterial { BaseColor = new(0.12f, 0.45f, 0.15f), Roughness = 0.8f, Metallic = 0.1f };
var blue = new CpuMaterial { BaseColor = new(0.05f, 0.05f, 0.65f), Roughness = 0.8f, Metallic = 0.1f };
var white = new CpuMaterial { BaseColor = new(0.73f, 0.73f, 0.73f), Roughness = 0.8f, Metallic = 0.1f };
var light = new CpuMaterial { EmissionColor = new(1, 1, 1), EmissionStrength = 5 };
var mirror = new CpuMaterial { BaseColor = new(0.9f, 0.9f, 0.9f), Metallic = 1.0f, Roughness = 0.0f };
var glass = new CpuMaterial { BaseColor = new(1, 1, 1), Metallic = 0.0f, Roughness = 0.0f, Transmission = 1.0f, IOR = 1.5f };
var anisotropic_metal = new CpuMaterial { BaseColor = new(0.8f, 0.6f, 0.2f), Metallic = 1.0f, Roughness = 0.2f, Anisotropic = 0.8f };
var wax = new CpuMaterial { BaseColor = new(0.9f, 0.85f, 0.7f), Metallic = 0.0f, Roughness = 1.0f, Transmission = 0.0f, IOR = 1.3f };

var sceneList = new List<ISceneObject>();
float s = 1.0f;
sceneList.AddRange(SceneFactory.CreateQuad(new(-s, -s, s), new(-s, s, s), new(-s, s, -s), new(-s, -s, -s), red)); // Левая стена
sceneList.AddRange(SceneFactory.CreateQuad(new(s, -s, -s), new(s, s, -s), new(s, s, s), new(s, -s, s), blue)); // Правая стена
sceneList.AddRange(SceneFactory.CreateQuad(new(s, s, -s), new(-s, s, -s), new(-s, s, s), new(s, s, s), white)); // Потолок
sceneList.AddRange(SceneFactory.CreateQuad(new(s, -s, s), new(-s, -s, s), new(-s, -s, -s), new(s, -s, -s), white)); // Пол
sceneList.AddRange(SceneFactory.CreateQuad(new(s, -s, s), new(s, s, s), new(-s, s, s), new(-s, -s, s), white)); // Задняя стена
sceneList.AddRange(SceneFactory.CreateQuad(new(s, -s, -s), new(s, s, -s), new(-s, s, -s), new(-s, -s, -s), white)); // Передняя стена

sceneList.AddRange(SceneFactory.CreateQuad(new(0, s - 0.001f, 0.75f), 1.0f, light));

//sceneList.Add(new Sphere(new CpuVector3(-0.4f, -0.4f, 0.6f), 0.3f, mirror));
//sceneList.Add(new Sphere(new CpuVector3(-0.4f, 0.15f, 0.6f), 0.2f, mirror));
//sceneList.Add(new Sphere(new CpuVector3(-0.4f, 0.5f, 0.6f), 0.1f, mirror));

sceneList.Add(new Sphere(new CpuVector3(0.4f, -0.4f, 0.6f), 0.3f, glass));
//sceneList.Add(new Sphere(new CpuVector3(0.4f, 0.15f, 0.6f), 0.2f, glass));
//sceneList.Add(new Sphere(new CpuVector3(0.4f, 0.5f, 0.6f), 0.1f, glass));

var stopwatch = new Stopwatch();
stopwatch.Start();
(CpuBvhNode[] cpuNodes, int[] orderedIndices) = LinearBvhBuilder.Build(sceneList);
stopwatch.Stop();
Console.WriteLine($"BVH построен за: {stopwatch.ElapsedMilliseconds} мс");

var gpuBvhNodes = BvhConverter.ConvertNodes(cpuNodes);
var gpuSceneObjects = BvhConverter.ConvertScene(sceneList, orderedIndices);

GraphicsDevice device = GraphicsDevice.GetDefault();
Console.WriteLine($"Используется устройство: {device.Name}");

using ReadOnlyBuffer<RaytracingShader.LinearBvhNode> gpuBvh = device.AllocateReadOnlyBuffer(gpuBvhNodes);
using ReadOnlyBuffer<RaytracingShader.SceneObject> gpuScene = device.AllocateReadOnlyBuffer(gpuSceneObjects);
using ReadWriteTexture2D<float4> outputTexture = device.AllocateReadWriteTexture2D<float4>(width, height);
using ReadWriteTexture2D<float4> accumulationBuffer = device.AllocateReadWriteTexture2D<float4>(width, height);

var camOrigin = new CpuVector3(0, 0, -0.999f);
var camDirection = CpuVector3.Normalize(new CpuVector3(0, -0.1f, 1));
var viewUp = new CpuVector3(0, 1, 0);
float verticalFov = 70.0f;
Float3 gpuCamOrigin = new Float3(camOrigin.X, camOrigin.Y, camOrigin.Z);
Float3 gpuCamDirection = new Float3(camDirection.X, camDirection.Y, camDirection.Z);
Float3 gpuViewUp = new Float3(viewUp.X, viewUp.Y, viewUp.Z);

Console.WriteLine($"Запуск трассировки ({totalSamplesPerPixel} spp, {batchSize} spp/batch)...");
stopwatch.Restart();

uint totalBatches = totalSamplesPerPixel / batchSize;
if (totalSamplesPerPixel % batchSize != 0) totalBatches++;

uint numTilesX = (uint)((width + tileSize - 1) / tileSize);
uint numTilesY = (uint)((height + tileSize - 1) / tileSize);


uint totalWorkItems = totalBatches * numTilesX * numTilesY;
uint completedWorkItems = 0;

var lastUpdateTime = Stopwatch.GetTimestamp();

device.For(width, height, new ClearShader(accumulationBuffer));

for (uint currentBatch = 0; currentBatch < totalBatches; currentBatch++)
{
    // Определяем, сколько сэмплов в текущем пакете (для последнего пакета может быть меньше)
    uint samplesInThisBatch = (currentBatch == totalBatches - 1 && totalSamplesPerPixel % batchSize != 0)
        ? totalSamplesPerPixel % batchSize
        : batchSize;

    for (uint tileY = 0; tileY < numTilesY; tileY++)
    {
        for (uint tileX = 0; tileX < numTilesX; tileX++)
        {
            uint offsetX = tileX * tileSize;
            uint offsetY = tileY * tileSize;

            int currentTileWidth = (int)Math.Min(tileSize, width - offsetX);
            int currentTileHeight = (int)Math.Min(tileSize, height - offsetY);

            var shader = new RaytracingShader(
                accumulationBuffer,
                gpuBvh,
                gpuScene,
                gpuCamOrigin,
                gpuCamDirection,
                gpuViewUp,
                verticalFov,
                samplesInThisBatch, // <--- Передаем точное число сэмплов для пакета
                maxBounces,
                (int)offsetX,
                (int)offsetY,
                currentBatch
                );

            device.For(currentTileWidth, currentTileHeight, shader);

            completedWorkItems++;

            // Обновляем прогресс-бар не слишком часто, чтобы не замедлять рендер
            var now = Stopwatch.GetTimestamp();
            if (Stopwatch.GetElapsedTime(lastUpdateTime, now).TotalMilliseconds > 200 || completedWorkItems == totalWorkItems)
            {
                double percentComplete = (double)completedWorkItems / totalWorkItems * 100;
                var elapsed = stopwatch.Elapsed;
                var estimatedTotalTime = elapsed / (percentComplete / 100);
                var remainingTime = estimatedTotalTime - elapsed;

                // Расчет производительности
                // Общее кол-во первичных лучей = ширина * высота * кол-во сэмплов до текущего момента
                long totalPrimaryRays = (long)width * height * (currentBatch * batchSize + samplesInThisBatch);
                double mRaysPerSecond = totalPrimaryRays / elapsed.TotalSeconds / 1_000_000;

                Console.Write($"\rProgress: {percentComplete:F1}% | " +
                              $"Elapsed: {elapsed:hh\\:mm\\:ss} | " +
                              $"ETA: {remainingTime:hh\\:mm\\:ss} | " +
                              $"Perf: {mRaysPerSecond:F2} MRays/s   ");

                lastUpdateTime = now;
            }
        }
    }
}

var finalizerShader = new FinalizeShader(accumulationBuffer, outputTexture, totalSamplesPerPixel);
device.For(width, height, finalizerShader);

stopwatch.Stop();
Console.WriteLine($"\nТрассировка завершена за: {stopwatch.Elapsed.TotalSeconds:F2} с.");

var pixelArray = new float4[height, width];
outputTexture.CopyTo(pixelArray);

using var image = new Image<Rgba32>(width, height);
for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        var pixel = pixelArray[y, x];
        image[x, y] = new Rgba32(
            (byte)(pixel.X * 255.999f),
            (byte)(pixel.Y * 255.999f),
            (byte)(pixel.Z * 255.999f), 255);
    }
}
image.SaveAsPng(outputPath);