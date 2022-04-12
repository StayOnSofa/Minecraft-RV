using BlockUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChunkUtils
{
    public interface IChunk
    {
        public Vector3 GetPosition();
        public Vector2Int GetChunkCoordinates();
        public bool BreakBlock(int x, int y, int z);
        public bool PlaceBlock(int x, int y, int z, int blockId);
        public int GetBlock(int x, int y, int z);
        public bool HasBlock(int x, int y, int z);
        public void Update(int x, int y, int z);
        public UnityEvent<Vector3Int> OnBlockChange();
        public UnityEvent<Vector3Int, int> OnBlockPlace();
        public UnityEvent<Vector3Int, int> OnBlockBreak();

        public UnityEvent OnChunkDestroyed();
        public void Tick(int tick);
        public void Destroy();

        public BlockHandler GetBlockHandler();
    }
}