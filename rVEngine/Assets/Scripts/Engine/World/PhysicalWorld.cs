using BiomeUtils;
using BlockUtils;
using ChunkUtils;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace WorldUtils
{
    public class PhysicalWorld : IWorld
    {
        private int _ticks = 0;
        private BlockHandler _blockHandler;

        private const int _chunkLoaderRadius = 6;
        private const float _mTimerTaskValue = 3f;

        private Dictionary<Vector2Int, IChunk> _chunks;
        private List<IChunkLoader> _chunkLoaders;
        private List<IChunk> _applicantChunksToRemove;

        private Dictionary<IChunkLoader, IEnumerable<IChunk>> _loadersNeedToAccept;

        private float _timerTask = _mTimerTaskValue;

        private bool _threadIsRunning = false;

        private BiomeHandler _biomeHandler;

        public PhysicalWorld()
        {
            _blockHandler = new BlockHandler();

            _biomeHandler = new BiomeHandler();
            _biomeHandler.Init();

            _chunks = new Dictionary<Vector2Int, IChunk>();
            _loadersNeedToAccept = new Dictionary<IChunkLoader, IEnumerable<IChunk>>();

            _chunkLoaders = new List<IChunkLoader>();
            _applicantChunksToRemove = new List<IChunk>();
        }

        public IEnumerable<IChunk> GetAllChunks()
        {
            return _chunks.Values;
        }
        public void AddLoader(IChunkLoader loader)
        {
            _chunkLoaders.Add(loader);
        }

        public void RemoveLoader(IChunkLoader loader)
        {
            if (_chunkLoaders.Contains(loader))
            {
                _chunkLoaders.Remove(loader);
            }
        }

        public Vector2Int ToChunkCoordinates(int globalX, int globalZ)
        {
            int chunkX = (int)Mathf.Floor(globalX / PhysicalChunk.Size.x);
            int chunkZ = (int)Mathf.Floor(globalZ / PhysicalChunk.Size.z);

            return new Vector2Int(chunkX, chunkZ);
        }

        public Vector2Int ToChunkBlockCoordinates(int globalX, int globalZ)
        {
            Vector2Int chunkCoordinates = ToChunkCoordinates(globalX, globalZ);

            int blockX = (int)(globalX - (chunkCoordinates.x * PhysicalChunk.Size.x));   
            int blockZ = (int)(globalZ - (chunkCoordinates.y * PhysicalChunk.Size.z));

            return new Vector2Int(blockX, blockZ);
        }
        public bool HasChunkGlobal(int globalX, int globalZ)
        {
            Vector2Int chunkCoordinates = ToChunkCoordinates(globalX, globalZ);

            if (_chunks.ContainsKey(chunkCoordinates))
            {
                return true;
            }

            return false;
        }

        public bool HasChunkLocal(int localX, int localY)
        {
            Vector2Int chunkCoordinates = new Vector2Int(localX, localY);

            if (_chunks.ContainsKey(chunkCoordinates))
            {
                return true;
            }

            return false;
        }

        public IChunk GetChunkOrCreateGlobal(int globalX, int globalZ)
        {
            Vector2Int chunkCoordinates = ToChunkCoordinates(globalX, globalZ);

            if (HasChunkGlobal(globalX, globalZ))
            {
                return _chunks[chunkCoordinates];
            }

            PhysicalChunk chunk = new PhysicalChunk(chunkCoordinates, this, _biomeHandler);

            if (!_chunks.ContainsKey(chunkCoordinates))
            {
                _chunks.Add(chunkCoordinates, chunk);
            }

            return chunk;
        }

        public IChunk GetChunkOrCreateLocal(int localX, int localZ)
        {
            Vector2Int chunkCoordinates = new Vector2Int(localX, localZ);

            if (HasChunkLocal(localX, localZ))
            {
                return _chunks[chunkCoordinates];
            }

            PhysicalChunk chunk = new PhysicalChunk(chunkCoordinates, this, _biomeHandler);
            if (!_chunks.ContainsKey(chunkCoordinates))
            {
                _chunks.Add(chunkCoordinates, chunk);
            }
            return chunk;
        }

        private void UpdateNeighborsChunks(int globalX, int globalY, int globalZ)
        {
            IChunk chunk = GetChunkOrCreateGlobal(globalX, globalZ);
            chunk.Update(globalX, globalY, globalZ);
        }

        private void UpdateNeigborsAround(int globalX, int globalY, int globalZ)
        {
            UpdateNeighborsChunks(globalX + 1, globalY, globalZ);
            UpdateNeighborsChunks(globalX - 1, globalY, globalZ);

            UpdateNeighborsChunks(globalX, globalY, globalZ + 1);
            UpdateNeighborsChunks(globalX, globalY, globalZ - 1);
        }

        public bool BreackBlock(int globalX, int globalY, int globalZ)
        {
            IChunk chunk = GetChunkOrCreateGlobal(globalX, globalZ);
            Vector2Int blockPosition = ToChunkBlockCoordinates(globalX, globalZ);
            
            int x = blockPosition.x;
            int y = globalY;
            int z = blockPosition.y;

            bool hasBreak = false;

            if (y >= 0 && y < PhysicalChunk.Size.y)
            {
                hasBreak = chunk.BreakBlock(x,y,z);
            }

            if (hasBreak)
            {
                int blockId = GetBlock(x, y, z);

                Block block = GetBlockHandler().GetBlock(blockId);
                block.BreakBlock(this, globalX, globalY, globalZ);

                UpdateNeigborsAround(globalX, globalY, globalZ);
            }

            return hasBreak;
        }


        public int GetBlock(int globalX, int globalY, int globalZ)
        {
            IChunk chunk = GetChunkOrCreateGlobal(globalX, globalZ);
            Vector2Int blockPosition = ToChunkBlockCoordinates(globalX, globalZ);

            int x = blockPosition.x;
            int y = globalY;
            int z = blockPosition.y;

            if (y >= 0 && y < PhysicalChunk.Size.y)
            {
                return chunk.GetBlock(x, y, z);
            }

            return BlockRegister.Air;
        }

        public bool HasBlock(int globalX, int globalY, int globalZ)
        {
            IChunk chunk = GetChunkOrCreateGlobal(globalX, globalZ);
            Vector2Int blockPosition = ToChunkBlockCoordinates(globalX, globalZ);

            int x = blockPosition.x;
            int y = globalY;
            int z = blockPosition.y;

            if (y >= 0 && y < PhysicalChunk.Size.y)
            {
                return chunk.HasBlock(x, y, z);
            }

            return false;
        }

        public bool PlaceBlock(int globalX, int globalY, int globalZ, int blockId)
        {
            IChunk chunk = GetChunkOrCreateGlobal(globalX, globalZ);
            Vector2Int blockPosition = ToChunkBlockCoordinates(globalX, globalZ);

            int x = blockPosition.x;
            int y = globalY;
            int z = blockPosition.y;

            bool hasPlace = false;

            if (y >= 0 && y < PhysicalChunk.Size.y)
            {
                hasPlace =  chunk.PlaceBlock(x, y, z, blockId);
            }

            if (hasPlace)
            {
                Block block = GetBlockHandler().GetBlock(blockId);
                block.PlaceBlock(this, globalX, globalY, globalZ);

                UpdateNeigborsAround(globalX, globalY, globalZ);
            }

            return hasPlace;
        }

        public IEnumerable<IChunk> GetChunksFromActiveChunkLoader(IChunkLoader chunkLoader, int radius)
        {
            IChunk[] chunks = new IChunk[((radius-1) + (radius-1)) * ((radius-1) + (radius-1))];

            Vector2Int stayInChunkPosition = ToChunkCoordinates(
                (int)chunkLoader.GetPosition().x, 
                (int)chunkLoader.GetPosition().z);

            int index = 0;

            for (int x = -radius; x < radius; x++)
            {
                for (int z = -radius; z < radius; z++)
                {
                    int _x = stayInChunkPosition.x + x;
                    int _z = stayInChunkPosition.y + z;

                    IChunk chunk = GetChunkOrCreateLocal(_x, _z);

                    DeleteFromRemoveList(chunk);

                    if (x > -radius && x < radius - 1)
                    {
                        if (z > -radius && z < radius - 1)
                        {
                            chunks[index] = chunk;
                            index++;
                        }
                    }
                }
            }

            return chunks;
        }

        public void Tick()
        {
            _timerTask += Time.deltaTime;
            if (_timerTask > _mTimerTaskValue)
            {
                TickEverySeconds();
                _timerTask = 0;
            }
        }

        private void TickEverySeconds()
        {
            if (_threadIsRunning == false)
            {
                ReloadLoaders();
                AcceptChunkLoaders();
                ClearUnusedChunksList();

                _ticks++;
                _threadIsRunning = true;

                new Thread(TickThreaded).Start();
            }
        }

        private void TickThreaded()
        {
            CollectChunkToRemoveList();
            TickChunkLoaders();

            _threadIsRunning = false;
        }

        private void CollectChunkToRemoveList()
        {
            foreach (IChunk chunk in _chunks.Values)
            {
                _applicantChunksToRemove.Add(chunk);
            }
        }

        private void DeleteFromRemoveList(IChunk chunk)
        {
            if (_applicantChunksToRemove.Contains(chunk))
            {
                _applicantChunksToRemove.Remove(chunk);
            }
        }

        private void ClearUnusedChunksList()
        {
            foreach (IChunk chunk in _applicantChunksToRemove)
            {
                chunk.Destroy();
                _chunks.Remove(chunk.GetChunkCoordinates());
            }

            _applicantChunksToRemove.Clear();
        }

        private void ReloadLoaders()
        {
            foreach (IChunkLoader loader in _chunkLoaders)
            {
                loader.Reload();
            }
        }

        private void AcceptChunkLoaders()
        {
            foreach (var item in _loadersNeedToAccept)
            {
                item.Key.Accept(item.Value);
            }

            _loadersNeedToAccept.Clear();
        }

        private void TickLoadedChunks(IEnumerable<IChunk> chunks)
        {
            foreach (IChunk chunk in chunks)
            {
                chunk.Tick(_ticks);
            }
        }

        private void TickChunkLoaders()
        {
            foreach (IChunkLoader loader in _chunkLoaders)
            {
                IEnumerable<IChunk> _chunks = GetChunksFromActiveChunkLoader(loader, _chunkLoaderRadius);
                TickLoadedChunks(_chunks);

                _loadersNeedToAccept.Add(loader, _chunks);
            }
        }

        public BlockHandler GetBlockHandler()
        {
            return _blockHandler;
        }

        public BiomeHandler GetBiomeHandler()
        {
            return _biomeHandler;
        }
    }
}
