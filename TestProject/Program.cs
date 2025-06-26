using TestProject;
using MathLibrary;
using MathLibrary.Tracing;
using TestProject.Renderer;

Console.WriteLine("--- Создание финальной PBR сцены (Cornell Box) ---");

// --- 1. Определение материалов ---
var lightMaterial = new Material { Emission = new Vector3(15) };
var glass = new Material { Transparency = 1.0f, IndexOfRefraction = 1.5f };
var mirror = new Material { Metallic = 1.0f, Roughness = 0.0f };
var redWall = new Material { Albedo = new Vector3(0.65f, 0.05f, 0.05f), Roughness = 1.0f };
var blueWall = new Material { Albedo = new Vector3(0.12f, 0.15f, 0.45f), Roughness = 1.0f };
var whiteWall = new Material { Albedo = new Vector3(0.73f), Roughness = 1.0f };

// --- 2. Создание объектов сцены ---
var sceneObjects = new List<ISceneObject>();

// Создаем Cornell Box. Размер комнаты 2x2x2, центр в (0,0,0)
var boxTriangles = new List<MeshTriangle>();
float s = 1.0f; // Половина размера стены
boxTriangles.AddRange(GeometryGenerator.GeneratePlane(new Vector3(0, -s, 0), 
    s * 2,Quaternion.Identity, whiteWall)); // Пол
boxTriangles.AddRange(GeometryGenerator.GeneratePlane(new Vector3(0, s, 0), 
    s * 2, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI), whiteWall)); // Потолок
boxTriangles.AddRange(GeometryGenerator.GeneratePlane(new Vector3(0, 0, -s), 
    s * 2, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2), whiteWall)); // Задняя стена
boxTriangles.AddRange(GeometryGenerator.GeneratePlane(new Vector3(0, 0, s),
    s * 2, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2), mirror)); // Передняя стена
boxTriangles.AddRange(GeometryGenerator.GeneratePlane(new Vector3(-s, 0, 0), 
    s * 2, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2), redWall)); // Левая стена
boxTriangles.AddRange(GeometryGenerator.GeneratePlane(new Vector3(s, 0, 0), 
    s * 2, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -MathF.PI / 2), blueWall)); // Правая стена

// Источник света на потолке
boxTriangles.AddRange(GeometryGenerator.GeneratePlane(new Vector3(0, s - 0.01f, 0), 
    1.0f, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI), lightMaterial));

// Добавляем всю геометрию коробки как один сложный объект с BVH
sceneObjects.Add(new MeshObject(boxTriangles));

sceneObjects.Add(new SphereObject(new Vector3(0.5f, -s + 0.4f, 0.2f), 0.4f, glass));
sceneObjects.Add(new SphereObject(new Vector3(-0.5f, -s + 0.4f, -0.3f), 0.4f, mirror));

Console.WriteLine($"  Сгенерирована сцена из {sceneObjects.Count} сложных объектов.");

// --- 3. Настройка камеры ---
var lookFrom = new Vector3(0, 0, 1.0f);
var lookAt = Vector3.Zero;
var vUp = Vector3.UnitY;
float verticalFov = 120.0f;
float aspectRatio = 1.0f;
var camera = new Camera(lookFrom, lookAt, vUp, verticalFov, aspectRatio);

// --- 4. Настройка рендеринга ---
var config = new RenderConfig
{
    ImageWidth = 1200,
    ImageHeight = 1200,
    SamplesPerPixel = 1024,
    MaxDepth = 8,
    OutputFileName = "TheGrandFinale.png"
};

// --- 5. Запуск ---
var renderer = new ImageRenderer(config, camera, sceneObjects);
renderer.RenderScene();
