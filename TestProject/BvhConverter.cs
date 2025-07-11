using ComputeSharp;
using MathLibrary.BVH;
using MathLibrary.Geometry;

namespace TestProject;

public static class BvhConverter
{
    public static RaytracingShader.LinearBvhNode[] ConvertNodes(CpuBvhNode[] cpuNodes)
    {
        var gpuNodes = new RaytracingShader.LinearBvhNode[cpuNodes.Length];
        for (int i = 0; i < cpuNodes.Length; i++)
        {
            var cpuNode = cpuNodes[i];
            gpuNodes[i] = new RaytracingShader.LinearBvhNode
            {
                BoundingBox = new RaytracingShader.Box
                {
                    Min = ToGpuVector(cpuNode.BoundingBox.Min),
                    Max = ToGpuVector(cpuNode.BoundingBox.Max)
                },
                LeftChildIndex = cpuNode.LeftChildIndex,
                RightChildIndex = cpuNode.RightChildIndex,
                PrimitiveCount = cpuNode.PrimitiveCount,
                FirstPrimitiveIndex = cpuNode.FirstPrimitiveIndex
            };
        }
        return gpuNodes;
    }

    public static RaytracingShader.SceneObject[] ConvertScene(IReadOnlyList<ISceneObject> scene, int[] orderedIndices)
    {
        var gpuObjects = new RaytracingShader.SceneObject[scene.Count];
        for (int i = 0; i < orderedIndices.Length; i++)
        {
            int originalIndex = orderedIndices[i];
            var obj = scene[originalIndex];
            var mat = obj.GetMaterial();

            var gpuObj = new RaytracingShader.SceneObject();

            // Заполняем новую PBR-структуру материала
            gpuObj.Mat.BaseColor = ToGpuVector(mat.BaseColor);
            gpuObj.Mat.Metallic = mat.Metallic;
            gpuObj.Mat.Roughness = mat.Roughness;
            gpuObj.Mat.Transmission = mat.Transmission;
            gpuObj.Mat.IOR = mat.IOR;
            gpuObj.Mat.Anisotropic = mat.Anisotropic;
            gpuObj.Mat.Clearcoat = mat.Clearcoat;
            gpuObj.Mat.ClearcoatRoughness = mat.ClearcoatRoughness;
            gpuObj.Mat.EmissionColor = ToGpuVector(mat.EmissionColor);
            gpuObj.Mat.EmissionStrength = mat.EmissionStrength;

            if (obj.GetObjectData() is Sphere sphere)
            {
                gpuObj.Type = 1;
                gpuObj.SphereCenter = ToGpuVector(sphere.Center);
                gpuObj.SphereRadius = sphere.Radius;
                gpuObj.Tangent = ToGpuVector(new Vector3(1, 0, 0));
            }
            else if (obj.GetObjectData() is Triangle tri)
            {
                gpuObj.Type = 3;
                gpuObj.TriV0 = ToGpuVector(tri.V0);
                gpuObj.TriV1 = ToGpuVector(tri.V1);
                gpuObj.TriV2 = ToGpuVector(tri.V2);

                // Вычисляем касательную для треугольника
                Vector3 edge1 = tri.V1 - tri.V0;
                gpuObj.Tangent = ToGpuVector(Vector3.Normalize(edge1));
            }
            gpuObjects[i] = gpuObj;
        }
        return gpuObjects;
    }

    private static RaytracingShader.Vector3 ToGpuVector(Vector3 v)
    {
        return new RaytracingShader.Vector3 { X = v.X, Y = v.Y, Z = v.Z };
    }
}
