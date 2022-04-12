using ChunkUtils;
using Render.MeshUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoChunk : MonoBehaviour
{
    private List<MonoChunkSlice> _slices = new List<MonoChunkSlice>();
    private int _sliceCount = 0;

    public void Init(IChunk chunk)
    {
        transform.position = chunk.GetPosition();

        CreateLayerSlices(chunk);

        chunk.OnBlockChange().AddListener(ChunkUpdated);
        chunk.OnChunkDestroyed().AddListener(Destroy);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }

    private Vector2Int _chunkCoordintes;
    private void CreateLayerSlices(IChunk chunk)
    {
        _chunkCoordintes = chunk.GetChunkCoordinates();

        int value = (int)(PhysicalChunk.Size.y / PhysicalChunk.Slice.y);
        _sliceCount = value;

        for (int i = 0; i < value; i++)
        {
            GameObject _sliceMonoChunk = new GameObject("[Slice]");
            _sliceMonoChunk.transform.SetParent(transform);

            MonoChunkSlice _slice = _sliceMonoChunk.AddComponent<MonoChunkSlice>();

            _slice.Init(chunk, i);
            _slice.transform.position = chunk.GetPosition() + new Vector3(0, i * PhysicalChunk.Slice.y, 0);

            _slices.Add(_slice);
        }
    }

    private void ChunkUpdated(Vector3Int position)
    {
        int y = position.y;
        int localY = (int)(y / PhysicalChunk.Slice.y);

        _slices[localY].UpdateFlagChanges();

        if ((localY + 1) < _sliceCount)
        {
            _slices[localY + 1].UpdateFlagChanges();
        }

        if ((localY - 1) >= 0)
        {
            _slices[localY - 1].UpdateFlagChanges();
        }
    }

    public Vector2Int GetChunkCoordinates()
    {
        return _chunkCoordintes;
    }

    public IEnumerable<MonoChunkSlice> GetSlices()
    {
        return _slices;
    }

    private void Destroy()
    {
        foreach (var slice in _slices)
        {
            Destroy(slice.gameObject);
        }

        Destroy(this.gameObject);
    }
}
