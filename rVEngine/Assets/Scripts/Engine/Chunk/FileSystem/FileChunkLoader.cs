using ChunkUtils.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkUtils.FileSystem
{
    public class FileChunkLoader : MonoBehaviour
    {
        private static Vector2 s_SizeSaveRegion= new Vector2(10, 10);

        private string _worldNames;
        private ChunkCompresser _compresser = new ChunkCompresser();

        public FileChunkLoader(string worldName)
        {
            _worldNames = worldName;
        }

        public IChunk GetChunkFromFile(int globalX, int globalZ)
        {

            return null;
        }

    }
}
