using SickDev.BitPacking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldUtils;

namespace ChunkUtils.Compression {
    public class ChunkCompresser
    {
        private const int sBitsUnCompressedValues = 16;

        private class SplitData
        {
            public int[] blocks;
            public Dictionary<int, int> blockIds;

            public SplitData(int[] blocks, Dictionary<int,int> blockIds)
            {
                this.blocks = blocks;
                this.blockIds = blockIds;
            }
        }

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

        private SplitData SplitChunkData(IChunk chunk)
        {
            int maxArraySize = (int)(PhysicalChunk.Size.x * PhysicalChunk.Size.y * PhysicalChunk.Size.z);
            int[] data = new int[maxArraySize];

            Dictionary<int, int> dictionaryIds = new Dictionary<int, int>();
           
            int indexDictionary = 0;
            int indexValue = 0;

            for (int y = 0; y < PhysicalChunk.Size.y; y++)
            {
                for (int x = 0; x < PhysicalChunk.Size.x; x++)
                {
                    for (int z = 0; z < PhysicalChunk.Size.z; z++)
                    {
                        int value = chunk.GetBlock(x, y, z);

                        if (!dictionaryIds.ContainsKey(value))
                        {
                            dictionaryIds.Add(value, indexDictionary);
                            indexDictionary++;
                        }

                        data[indexValue] = value;
                        indexValue++;
                    }
                }
            }

            return new SplitData(data, dictionaryIds);
        }

        private int[,,] CompileChunkData(int[] blocks)
        {
            int[,,] chunkBlocks = new int[(int)PhysicalChunk.Size.x, (int)PhysicalChunk.Size.y, (int)PhysicalChunk.Size.z];
            int indexValue = 0;

            for (int y = 0; y < PhysicalChunk.Size.y; y++)
            {
                for (int x = 0; x < PhysicalChunk.Size.x; x++)
                {
                    for (int z = 0; z < PhysicalChunk.Size.z; z++)
                    {
                        int value = blocks[indexValue];
                        chunkBlocks[x, y, z] = value;

                        indexValue++;
                    }
                }
            }

            return chunkBlocks;
        }

        private void WriteBlockIds(BitWriter writer, SplitData splitData, int bits)
        {
            writer.Write(splitData.blockIds.Count, bits);

            foreach (var item in splitData.blockIds)
            {
                int value = item.Key;

                writer.Write(Math.Abs(value), bits);
                writer.Write(value < 0 ? 0 : 1, 1);
            }
        }

        private void WriteChunkBlocks(BitWriter writer, SplitData splitData)
        {
            int maxArraySize = (int)(PhysicalChunk.Size.x * PhysicalChunk.Size.y * PhysicalChunk.Size.z);

            int[] blocks = splitData.blocks;
            Dictionary<int, int> blocksIds = splitData.blockIds;

            int bits = GetNumbersOfBits(0, blocksIds.Count);
            writer.Write(bits, sBitsUnCompressedValues);

            for (int i = 0; i < maxArraySize; i++)
            {
                int value = blocks[i];
                int convertValue = blocksIds[value];

                writer.Write(Math.Abs(convertValue), bits);
                writer.Write(convertValue < 0 ? 0 : 1, 1);
            }
        }

        private List<int> ReadBlockIds(BitReader reader, int bits)
        {
            List<int> rBlockIds = new List<int>();
            int count = (int)reader.Read(bits);

            for (int i = 0; i < count; i++)
            {
                int value = (int)reader.Read(bits);
            
                if (reader.Read(1) == 0)
                    value *= -1;

                rBlockIds.Add(value);
            }

            return rBlockIds;
        }

        private int[] ReadChunkBlocks(BitReader reader, List<int> blockIds)
        {
            int maxArraySize = (int)(PhysicalChunk.Size.x * PhysicalChunk.Size.y * PhysicalChunk.Size.z);
            int[] blocks = new int[maxArraySize];

            int bits = (int)reader.Read(sBitsUnCompressedValues);

            for (int i = 0; i < maxArraySize; i++)
            {
                int value = (int)reader.Read(bits);

                if (reader.Read(1) == 0)
                    value *= -1;

                blocks[i] = blockIds[value];
            }

            return blocks;
        }

        public byte[] CompressChunk(IChunk chunk)
        {
            SplitData splitData = SplitChunkData(chunk);
            BitWriter writer = new BitWriter();

            WriteBlockIds(writer, splitData, sBitsUnCompressedValues);
            WriteChunkBlocks(writer, splitData);

            byte[] rawBytes = writer.GetBytes();
            byte[] compressedBytes = SevenZip.Compression.LZMA.SevenZipHelper.Compress(rawBytes);


            return compressedBytes;
        }

        public byte[] TestUnrealisticSituation(IChunk chunk, int maxId)
        {
            SplitData splitData = SplitChunkData(chunk);
            int[] unrealBlocks = new int[32 * 32 * 256];
            int index = 0;


            Dictionary<int, int> dictionaryIds = new Dictionary<int, int>();

            int indexDictionary = 0;

            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    for (int z = 0; z < 32; z++)
                    {
                        int value = UnityEngine.Random.Range(-maxId, maxId);
                        unrealBlocks[index] = value;

                        if (!dictionaryIds.ContainsKey(value))
                        {
                            dictionaryIds.Add(value, indexDictionary);
                            indexDictionary++;
                        }

                        index++;
                    }
                }
            }

            splitData.blocks = unrealBlocks;
            splitData.blockIds = dictionaryIds;

            Debug.Log("Dictinory: " + dictionaryIds.Values.Count);
            Debug.Log(GetNumbersOfBits(0, 4000));

            BitWriter writer = new BitWriter();

            WriteBlockIds(writer, splitData, sBitsUnCompressedValues);
            WriteChunkBlocks(writer, splitData);

            byte[] rawBytes = writer.GetBytes();
            byte[] compressedBytes = SevenZip.Compression.LZMA.SevenZipHelper.Compress(rawBytes);

            Debug.Log(rawBytes.Length);

            return compressedBytes;
        }

        public int[,,] UnCompressChunk(byte[] compressedBytes)
        {
            byte[] deCompressedBytes = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(compressedBytes);

            BitReader reader = new BitReader(deCompressedBytes);

            List<int> blockIds = ReadBlockIds(reader, sBitsUnCompressedValues);
            int[] blocks = ReadChunkBlocks(reader, blockIds);

            return CompileChunkData(blocks);
        }
    }
}