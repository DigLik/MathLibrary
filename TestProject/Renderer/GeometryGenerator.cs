using MathLibrary;
using MathLibrary.Geometry;
using MathLibrary.Tracing;

namespace TestProject.Renderer;

public static class GeometryGenerator
{
    public static List<MeshTriangle> GenerateSphere(Vector3 center, float radius, int longSegments, int latSegments, Material material)
    {
        var meshTriangles = new List<MeshTriangle>();
        var vertices = new Vector3[longSegments + 1, latSegments + 1];
        var normals = new Vector3[longSegments + 1, latSegments + 1];

        // 1. Генерируем сетку вершин и их нормалей
        for (var j = 0; j <= latSegments; j++)
        {
            for (var i = 0; i <= longSegments; i++)
            {
                var u = (float)i / longSegments;
                var v = (float)j / latSegments;
                var theta = u * 2 * MathF.PI;
                var phi = v * MathF.PI;

                var position = new Vector3(
                    center.X + radius * MathF.Sin(phi) * MathF.Cos(theta),
                    center.Y + radius * MathF.Cos(phi),
                    center.Z + radius * MathF.Sin(phi) * MathF.Sin(theta)
                );
                vertices[i, j] = position;

                // Для идеальной сферы нормаль в точке на поверхности - это просто
                // нормализованный вектор от центра сферы к этой точке.
                normals[i, j] = Vector3.Normalize(position - center);
            }
        }

        // 2. Собираем MeshTriangle из вершин и нормалей
        for (var j = 0; j < latSegments; j++)
        {
            for (var i = 0; i < longSegments; i++)
            {
                var v1 = vertices[i, j]; var n1 = normals[i, j];
                var v2 = vertices[i + 1, j]; var n2 = normals[i + 1, j];
                var v3 = vertices[i, j + 1]; var n3 = normals[i, j + 1];
                var v4 = vertices[i + 1, j + 1]; var n4 = normals[i + 1, j + 1];

                // Создаем два треугольника, формирующих четырехугольник на сетке
                meshTriangles.Add(new MeshTriangle(new Triangle(v1, v3, v4), n1, n3, n4, material));
                meshTriangles.Add(new MeshTriangle(new Triangle(v1, v4, v2), n1, n4, n2, material));
            }
        }
        return meshTriangles;
    }

    public static List<MeshTriangle> GeneratePlane(Vector3 center, float size, Quaternion rotation, Material material)
    {
        var half = size / 2.0f;

        // Определяем вершины в локальном пространстве (до вращения и сдвига)
        var p1 = new Vector3(-half, 0, -half);
        var p2 = new Vector3(half, 0, -half);
        var p3 = new Vector3(-half, 0, half);
        var p4 = new Vector3(half, 0, half);

        // Вращаем вершины и затем смещаем их в нужную точку
        var v1 = Vector3.Transform(p1, rotation) + center;
        var v2 = Vector3.Transform(p2, rotation) + center;
        var v3 = Vector3.Transform(p3, rotation) + center;
        var v4 = Vector3.Transform(p4, rotation) + center;

        // Нормаль также нужно повернуть
        var normal = Vector3.Transform(Vector3.UnitY, rotation);

        return
            [
                new(new Triangle(v1, v2, v4), normal, normal, normal, material),
                new(new Triangle(v1, v4, v3), normal, normal, normal, material)
            ];
    }

    public static List<MeshTriangle> GenerateCornellBox(float size, Material wallMaterial)
    {
        var box = new List<MeshTriangle>();
        var half = size / 2.0f;

        // Пол
        box.AddRange(GeneratePlane(new Vector3(0, -half, 0), size, Quaternion.Identity, wallMaterial));
        // Потолок
        box.AddRange(GeneratePlane(new Vector3(0, half, 0), size, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI), wallMaterial));
        // Задняя стена
        box.AddRange(GeneratePlane(new Vector3(0, 0, -half), size, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2), wallMaterial));
        // Левая стена (красная)
        var redMat = wallMaterial with { Albedo = new Vector3(0.8f, 0.1f, 0.1f) };
        box.AddRange(GeneratePlane(new Vector3(-half, 0, 0), size, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2), redMat));
        // Правая стена (синяя)
        var blueMat = wallMaterial with { Albedo = new Vector3(0.1f, 0.2f, 0.8f) };
        box.AddRange(GeneratePlane(new Vector3(half, 0, 0), size, Quaternion.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2), blueMat));

        return box;
    }
}
