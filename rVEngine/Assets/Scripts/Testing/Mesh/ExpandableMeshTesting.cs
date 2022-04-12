using Render.MeshUtils;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ExpandableMeshTesting : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;

    [BurstCompile]
    private struct MeshData
    {
        public NativeList<Vector3> vertices;
        public NativeList<Vector2> uvs;
        public NativeList<Vector3> normals;
        public NativeList<int> triangles;

        public void Dispose()
        {
            vertices.Dispose();
            uvs.Dispose();
            normals.Dispose();
            triangles.Dispose();
        }
    }

    private MeshData MeshDataFromMesh(Mesh mesh)
    {
        NativeList<Vector3> vertices = new NativeList<Vector3>(Allocator.TempJob);

        foreach (Vector3 item in mesh.vertices)
        {
            vertices.Add(new Vector3(item.x, item.y, item.z));
        }

        NativeList<Vector2> uvs = new NativeList<Vector2>(Allocator.TempJob);
        foreach (Vector2 item in mesh.uv)
        {
            uvs.Add(new Vector2(item.x, item.y));
        }

        NativeList<Vector3> normals = new NativeList<Vector3>(Allocator.TempJob);
        foreach (Vector3 item in mesh.normals)
        {
            normals.Add(new Vector3(item.x, item.y, item.z));;
        }

        NativeList<int> triangles = new NativeList<int>(Allocator.TempJob);

        foreach (int item in mesh.triangles)
        {
            triangles.Add(item);
        }

        MeshData meshData = new MeshData();

        meshData.vertices = vertices;
        meshData.uvs = uvs;
        meshData.normals = normals;
        meshData.triangles = triangles;
        
        return meshData;
    }

    private void ApplyMeshFromMeshList(Mesh mesh, MeshList list)
    {
        mesh.Clear();

        Vector3[] _vertices = new Vector3[list.vertices.Length];
        for (int i = 0; i < list.vertices.Length; i++)
        {
            Vector3 item = list.vertices[i];
            _vertices[i] = new Vector3(item.x, item.y, item.z);
        }

        Vector2[] _uvs = new Vector2[list.uvs.Length];
        for (int i = 0; i < list.uvs.Length; i++)
        {
            Vector2 item = list.uvs[i];
            _uvs[i] = new Vector2(item.x, item.y);
        }


        Vector3[] _normals = new Vector3[list.normals.Length];
        for (int i = 0; i < list.normals.Length; i++)
        {
            Vector3 item = list.normals[i];
            _normals[i] = new Vector3(item.x, item.y, item.z);
        }

        int[] _triangles = new int[list.triangles.Length];
        for (int i = 0; i < list.triangles.Length; i++)
        {
            int item = list.triangles[i];
            _triangles[i] = item;
        }

        mesh.vertices = _vertices;
        mesh.uv = _uvs;
        mesh.normals = _normals;
        mesh.triangles = _triangles;
    }

    private MeshList AddMeshList()
    {
        MeshList meshList = new MeshList();

        meshList.vertices = new NativeList<Vector3>(Allocator.TempJob);
        meshList.uvs = new NativeList<Vector2>(Allocator.TempJob);
        meshList.normals = new NativeList<Vector3>(Allocator.TempJob);
        meshList.triangles = new NativeList<int>(Allocator.TempJob);

        return meshList;
    }

    [BurstCompile]
    private struct MeshList
    {
        public NativeList<Vector3> vertices;
        public NativeList<Vector2> uvs;
        public NativeList<Vector3> normals;
        public NativeList<int> triangles;

        public void Dispose()
        {
            vertices.Dispose();
            uvs.Dispose();
            normals.Dispose();
            triangles.Dispose();
        }
    }

    [BurstCompile]
    private struct MeshExpands : IJob
    {
        public MeshData mesh;
        public MeshList list;
        public int radius;

        public void AddMesh(Vector3 position)
        {
            int pastVerticeCount = list.vertices.Length;

            foreach (Vector3 vertice in mesh.vertices)
            {
                list.vertices.Add((vertice * 0.5f) + position);
            }

            foreach (Vector3 normal in mesh.normals)
            {
                list.normals.Add(normal);
            }

            foreach (Vector2 uv in mesh.uvs)
            {
                list.uvs.Add(uv);
            }

            foreach (int triangle in mesh.triangles)
            {
                int id = pastVerticeCount + triangle;
                list.triangles.Add(id);
            }
        }

        public void Execute()
        {
            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    for (int z = 0; z < radius; z++)
                    {
                        AddMesh(new Vector3(x, y, z));
                    }
                }
            }
        }
    }

    private void Start()
    {

    }

    private void BuildMesh()
    {
        MeshList meshList = AddMeshList();
        MeshData meshData = MeshDataFromMesh(_mesh);

        MeshExpands jobTask = new MeshExpands
        {
            mesh = meshData,
            list = meshList,
            radius = Random.Range(1, 25),
        };

        JobHandle jobHandle = jobTask.Schedule();
        jobHandle.Complete();

        Mesh mesh = new Mesh();

        ApplyMeshFromMeshList(mesh, meshList);
        GetComponent<MeshFilter>().mesh = mesh;

        meshList.Dispose();
        meshData.Dispose();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            BuildMesh();
        }
    }
}
