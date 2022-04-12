using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;
using emotitron.Compression;
using SickDev.BitPacking;
using ChunkUtils;
using ChunkUtils.Compression;

public class BitsTestingCompression : MonoBehaviour
{
    private int GetNumbersOfBits(int min, int max)
    {
        int bits = 1;
        bool answer = false;
        int addValue = 0;

        if (min <= 0)
        {
            addValue = Mathf.Abs(min);
        }

        while (!answer)
        {
            int currentBits = (int)Mathf.Pow(2, bits);

            if (!(currentBits >= ((max + addValue))))
            {
                bits++;
            }
            else
            {
                answer = true;
            }
        }

        return bits;
    }

    ChunkCompresser compresser = new ChunkCompresser();
    private void TestingArrays()
    {
        PhysicalWorld world = new PhysicalWorld();
        IChunk chunk = world.GetChunkOrCreateGlobal(0,0);

        byte[] bytes = compresser.CompressChunk(chunk);
        Debug.Log(bytes.Length);

        byte[] unrealBytes = compresser.TestUnrealisticSituation(chunk, 512);
        Debug.Log(unrealBytes.Length);
    }

    private bool CompressionTesting(IChunk chunk)
    {
        byte[] compressedBytes = compresser.CompressChunk(chunk);
        int[,,] blocks = compresser.UnCompressChunk(compressedBytes);

        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 256; y++)
            {
                for (int z = 0; z < 32; z++)
                {
                    int value1 = blocks[x,y,z];
                    int value = chunk.GetBlock(x,y,z);

                    if (value1 != value)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }


    private void Start()
    {
        TestingArrays();
    }
}
