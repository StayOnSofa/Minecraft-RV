using BlockUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkUtils
{
    public class StructureTick
    {
        public int BlockId;

        public int X;
        public int Y;
        public int Z;

        public StructureTick(int x, int y, int z, int blockId)
        {
            BlockId = blockId;

            X = x;
            Y = y;
            Z = z;
        }
    }
}