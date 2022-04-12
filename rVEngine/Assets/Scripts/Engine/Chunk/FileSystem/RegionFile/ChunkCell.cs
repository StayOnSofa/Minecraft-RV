using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkUtils.FileSystem
{
    public struct ChunkCell
    {
        public int filesBytes;
        public byte[] compressedChunk;
        public int nextCell;
    }
}