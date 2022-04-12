using BiomeUtils;
using BlockUtils;
using ChunkUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldUtils
{
    public interface IWorld
    {
        public bool PlaceBlock(int x, int y, int z, int blockId);
        public bool BreackBlock(int x, int y, int z);
        public int GetBlock(int x, int y, int z);
        public bool HasBlock(int x, int y, int z);
        public void AddLoader(IChunkLoader loader);
        public void RemoveLoader(IChunkLoader loader);
        public BlockHandler GetBlockHandler();
    }
}
