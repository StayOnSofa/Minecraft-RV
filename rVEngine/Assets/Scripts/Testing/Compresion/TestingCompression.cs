using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChunkUtils;
using WorldUtils;

public class TestingCompression : MonoBehaviour
{
    [SerializeField] private MonoChunk _monoChunk;

    private class SliceRegion
    {
        public byte[] chunk = new byte[1024];
    }

    private class Region
    {
        List<SliceRegion> Slices = new List<SliceRegion>();
    }

    IChunk chunk;

    private int[] GetChunkData()
    {
        PhysicalWorld world = new PhysicalWorld();

        List<int> values = new List<int>();

        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 32*16; x++)
            {
                for (int z = 0; z < 32*16; z++)
                {
                    int value = world.GetBlock(x,y,z);
                    values.Add(value);
                }
            }
        }

        return values.ToArray();
    }

    private void Start()
    {
        int[] values = GetChunkData();
        byte[] dataToCompresser = new byte[values.Length * 4];

        int byteIndex = 0;
        for (int i = 0; i < values.Length; i++)
        {
            byte[] intBytes = BitConverter.GetBytes(values[i]);

            for (int z = 0; z < 4; z++)
            {
                dataToCompresser[byteIndex] = intBytes[z];
                byteIndex++;
            }
        }


        byte[] compressed = SevenZip.Compression.LZMA.SevenZipHelper.Compress(dataToCompresser);
        Debug.Log(dataToCompresser.Length + ":" + compressed.Length);
    }
}
