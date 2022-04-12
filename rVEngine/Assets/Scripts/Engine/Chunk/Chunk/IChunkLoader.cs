using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkUtils
{
    public interface IChunkLoader
    {
        public void Reload();
        public Vector3 GetPosition();
        public void Accept(IEnumerable<IChunk> chunksLoad);
    }
}
