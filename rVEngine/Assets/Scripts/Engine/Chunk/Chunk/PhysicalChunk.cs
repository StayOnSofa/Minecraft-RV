using BiomeUtils;
using BlockUtils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using WorldUtils;

namespace ChunkUtils {
    public class PhysicalChunk : IChunk
    {
        private static System.Random _random = new System.Random();

        private int _tick = 0;

        public static Vector3 Size = new Vector3Int(32, 256, 32);
        public static Vector3 Slice = Vector3Int.one * 32;

        private Dictionary<Vector3Int, TickBlock> _tickBlocks = new Dictionary<Vector3Int, TickBlock>();

        private List<StructureTick> _structureTicks = new List<StructureTick>();

        private IWorld _world;

        private int[,,] _data;

        private UnityEvent<Vector3Int, int> _onBlockBreak;
        private UnityEvent<Vector3Int, int> _onBlockPlace;

        private UnityEvent<Vector3Int> _onBlockChange;
        private UnityEvent _onChunkDestroyed;

        private Vector3 _position;
        private Vector2Int _chunkCoordinates;

        private BiomeHandler _biomeHandler;

        private static Stopwatch stopwatch = new Stopwatch();

        public PhysicalChunk(Vector2Int chunkCoordinates, IWorld world, BiomeHandler biomeHandler)
        {
            _world = world;

            _chunkCoordinates = chunkCoordinates;
            _position = new Vector3(_chunkCoordinates.x * Size.x, 0, _chunkCoordinates.y * Size.z);

            _data = new int[(int)Size.x, (int)Size.y, (int)Size.z];

            _onBlockBreak = new UnityEvent<Vector3Int, int>();
            _onBlockPlace = new UnityEvent<Vector3Int, int>();
            _onBlockChange = new UnityEvent<Vector3Int>();
            _onChunkDestroyed = new UnityEvent();

            _biomeHandler = biomeHandler;

            FillChunk();
        }

        private void StructureBlocksTick()
        {
            foreach (StructureTick tickBlock in _structureTicks)
            {
                Block block = GetBlockHandler().GetBlock(tickBlock.BlockId);
                block.PlaceBlock(_world, (int)_position.x + tickBlock.X, tickBlock.Y, (int)_position.z + tickBlock.Z);
            }

            _structureTicks.Clear();
        }

        private void AddTickBlock(int x, int y, int z, int blockId)
        {
            _structureTicks.Add(new StructureTick(x,y,z, blockId));
        }

        public void Tick(int tick)
        {
            if (_tick < tick)
            {
                StructureBlocksTick();
                TickBlocks(tick);

                _tick = tick;
            }
        }

        public int[,,] GetData()
        {
            return _data;
        }

        private void FillChunk()
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int z = 0; z < Size.z; z++)
                {
                    int X = (int)_position.x + x;
                    int Z = (int)_position.z + z;

                    Biome biome = _biomeHandler.GetPrimaryBiome(X, Z);
                    int height = _biomeHandler.GetMixedHeight(X, Z);

                    for (int y = 0; y <= height; y++)
                    {
                        int blockId = biome.GetBlock(X, y, Z, height);
                        Block block = _world.GetBlockHandler().GetBlock(blockId);

                        if (block is StructureBlock)
                        {
                            AddTickBlock(x,y,z, blockId);
                        }

                        _data[x, y, z] = blockId;
                    }
                }
            }
        }

        private bool InBorder(int value, int border)
        {
            if (value >= 0 && value < border)
            {
                return true;
            }

            return false;
        }
        public bool InBorder(int x, int y, int z)
        {
            if (InBorder(x, (int)Size.x) && InBorder(y, (int)Size.y) && InBorder(z, (int)Size.z))
            {
                return true;
            }

            return false;
        }

        public int GetBlock(int x, int y, int z)
        {
            if (InBorder(x, y, z))
            {
                return _data[x, y, z];
            }
            else
            {
                Vector3Int blockCoordinates = new Vector3Int((int)(_position.x + x), y, (int)(_position.z + z));
                return _world.GetBlock(blockCoordinates.x, blockCoordinates.y, blockCoordinates.z);
            }
        }

        public bool HasBlock(int x, int y, int z)
        {
            if (InBorder(x, y, z))
            {
                if (_data[x, y, z] != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void TickBlockPlaceCheck(Vector3Int position, int blockId)
        {
            Block block = GetBlockHandler().GetBlock(blockId);

            if (block is TickBlock)
            {
                _tickBlocks.Add(position, block as TickBlock);
            }
        }

        private void TickBlockBreakCheck(Vector3Int position, int blockId)
        {
            Block block = GetBlockHandler().GetBlock(blockId);

            if (block is TickBlock)
            {
                if (_tickBlocks.ContainsKey(position))
                {
                    _tickBlocks.Remove(position);
                }
            }
        }

        private void TickBlocks(int tick)
        {
            List<TickBlock> blocks = _tickBlocks.Values.ToList();
            List<Vector3Int> positions = _tickBlocks.Keys.ToList();

            if (blocks.Count > 0)
            {
                for (int _i = 0; _i < 3; _i++)
                {
                    int index = _random.Next(0, blocks.Count);

                    TickBlock block = blocks[index];
                    Vector3Int position = positions[index] + new Vector3Int((int)_position.x, 0, (int)_position.z);

                    if (block != null)
                    {
                        block.Tick(_world, position, tick);
                    }
                }
            }
        }


        public bool BreakBlock(int x, int y, int z)
        {
            if (HasBlock(x, y, z))
            {
                int blockId = _data[x, y, z];
                _data[x, y, z] = 0;

                Vector3Int blockPosition = new Vector3Int(x, y, z);
                TickBlockBreakCheck(blockPosition, blockId);

                _onBlockBreak?.Invoke(blockPosition, blockId);
                _onBlockChange?.Invoke(blockPosition);

                return true;
            }

            return false;
        }

        public bool PlaceBlock(int x, int y, int z, int blockId)
        {
            if (!HasBlock(x, y, z))
            {
                _data[x, y, z] = blockId;
                Vector3Int blockPosition = new Vector3Int(x, y, z);
                TickBlockPlaceCheck(blockPosition, blockId);

                _onBlockPlace?.Invoke(blockPosition, blockId);
                _onBlockChange?.Invoke(blockPosition);

                return true;
            }

            return false;
        }

        public void Update(int x, int y, int z)
        {
            Vector3Int blockPosition = new Vector3Int(x, y, z);
            _onBlockChange?.Invoke(blockPosition);
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public Vector2Int GetChunkCoordinates()
        {
            return _chunkCoordinates;
        }

        public UnityEvent<Vector3Int> OnBlockChange()
        {
            return _onBlockChange;
        }

        public UnityEvent<Vector3Int, int> OnBlockPlace()
        {
            return _onBlockPlace;
        }

        public UnityEvent<Vector3Int, int> OnBlockBreak()
        {
            return _onBlockBreak;
        }

        public UnityEvent OnChunkDestroyed()
        {
            return _onChunkDestroyed;
        }

        public void Destroy()
        {
            _onBlockBreak.RemoveAllListeners();
            _onBlockPlace.RemoveAllListeners();
            _onBlockChange.RemoveAllListeners();

            _onChunkDestroyed?.Invoke();
            _onChunkDestroyed.RemoveAllListeners();
        }

        public BlockHandler GetBlockHandler()
        {
            return _world.GetBlockHandler();
        }
    }
}