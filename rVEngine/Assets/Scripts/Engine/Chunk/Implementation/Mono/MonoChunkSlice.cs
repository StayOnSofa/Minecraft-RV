using ChunkUtils;
using ChunkUtils.Implementation;
using Render.MeshUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(Renderer))]
public class MonoChunkSlice : MonoBehaviour
{
    private static RenderChunksJob _renderJob;
    private static Material _material;
    private static void InitMaterial()
    {
        if (_material == null)
        {
            _material = Resources.Load<Material>("Block/Atlas/Atlas");
        }
    }

    private static void InitJob()
    {
        if (_renderJob == null)
        {
            _renderJob = Singleton<RenderChunksJob>.Instance();
        }
    }

    private int _layer = 0;

    private Renderer _renderer;
    private MeshFilter _meshFilter;
    private MeshChunk _meshChunk;

    private bool _flagBuilded = false;

    public void Init(IChunk chunk, int layer)
    {
        _meshChunk = new MeshChunk(chunk);
        _layer = layer;

        InitJob();
        InitMaterial();

        GetComponent<MeshRenderer>().material = _material;

        _renderer = GetComponent<Renderer>();
        _meshFilter = GetComponent<MeshFilter>();

        _meshFilter.mesh = _meshChunk.GetMeshWithCollider();

        _renderJob.AddSlice(this);

        AddChild();
    }

    private MonoWithoutCollideSlice _monoCollidless;
    private void AddChild()
    {
        GameObject gameObject = new GameObject("[collidless]");
        _monoCollidless = gameObject.AddComponent<MonoWithoutCollideSlice>();

        _monoCollidless.Init(_meshChunk, _material, transform);
    }

    public void UpdateFlagChanges()
    {
        _flagHasChanges = true;
    }

    private bool _flagHasChanges = false;

    public void BuildMesh()
    {
        _meshChunk.BuildMesh(_layer);

        _flagBuilded = true;
    }

    public void TryBuildMesh()
    {
        if (_flagBuilded)
        {
            if (_flagHasChanges)
            {
                BuildMesh();
                _flagHasChanges = false;
            }
        }
    }

    private float _timer = 0;
    private float _maxSliceGraphicsTime = 0.15f;

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer > _maxSliceGraphicsTime)
        {
            TryBuildMesh();
        }
    }


    public bool IsHasChangesPerLastMeshBuild()
    {
        return _flagHasChanges;
    }

    public bool IsBuilded()
    {
        return _flagBuilded;
    }


    public bool IsVisible()
    {
        return _renderer.isVisible;
    }

    private void OnDestroy()
    {
        _renderJob.RemoveSlice(this);
    }
}
