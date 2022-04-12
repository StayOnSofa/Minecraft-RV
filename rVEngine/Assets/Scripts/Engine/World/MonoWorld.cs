using BiomeUtils;
using BlockUtils;
using ChunkUtils;
using Render.Block;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldUtils
{
    public class MonoWorld : MonoBehaviour, IWorld
    {
        [SerializeField] private List<GameObject> _loaders;
        [SerializeField] private BlockMaterialLoader _blockMaterialLoader;

        private PhysicalWorld _localWorld;

        private void OnValidate()
        {
            for (int i = 0; i < _loaders.Count; i++)
            {
                GameObject loader = _loaders[i];

                IChunkLoader iLoader = loader.GetComponent<IChunkLoader>();
                if (iLoader == null)
                {
                    _loaders[i] = null;
                }
            }
        }

        private IEnumerable<IChunk> _chunks;

        private void Start()
        {
            _localWorld = new PhysicalWorld();
            for (int i = 0; i < _loaders.Count; i++)
            {
                IChunkLoader _chunkLoader = _loaders[i].GetComponent<IChunkLoader>();
                _localWorld.AddLoader(_chunkLoader);
            }


            _blockMaterialLoader.Init(_localWorld.GetBlockHandler());
        }
        private void FixedUpdate()
        {
            _localWorld.Tick();
        }

        public void AddLoader(IChunkLoader loader)
        {
            _localWorld.AddLoader(loader);
        }

        public bool BreackBlock(int x, int y, int z)
        {
            return _localWorld.BreackBlock(x, y, z);
        }

        public int GetBlock(int x, int y, int z)
        {
            return _localWorld.GetBlock(x, y, z);
        }

        public bool HasBlock(int x, int y, int z)
        {
            return _localWorld.HasBlock(x, y, z);
        }

        public bool PlaceBlock(int x, int y, int z, int blockId)
        {
            return _localWorld.PlaceBlock(x, y, z, blockId);
        }

        public void RemoveLoader(IChunkLoader loader)
        {
           _localWorld.RemoveLoader(loader);
        }

        public BlockHandler GetBlockHandler()
        {
            return _localWorld.GetBlockHandler();
        }
    }
}