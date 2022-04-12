using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkUtils.FileSystem
{
    public class RegionRegister
    {
        public const int cRegisterBytes = 2048;
        private const int cValueOffset = 8;

        private static int sCubicRegisterSize = 16;

        private byte[] data;
        public RegionRegister()
        {
            data = new byte[cRegisterBytes];

            int bytesIndex = 0;
            int index = 0;

            for (int x = 0; x < sCubicRegisterSize; x++)
            {
                for (int y = 0; y < sCubicRegisterSize; y++)
                {
                    WriteInt(ref bytesIndex, ref data, index);
                    WriteInt(ref bytesIndex, ref data, -1);

                    index++;
                }
            }
        }
        public RegionRegister(byte[] bytes)
        {
            data = bytes;
        }

        private void WriteInt(ref int bytesIndex, ref byte[] data, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            for (int i = 0; i < bytes.Length; i++)
            {
                data[bytesIndex] = bytes[i];
                bytesIndex++;
            }
        }

        public int GetIndex(int x, int y)
        {
            return y + (x * sCubicRegisterSize);
        }

        public int GetIdByIndex(int x, int y)
        {
            int offset = GetIndex(x, y) * cValueOffset;
            return BitConverter.ToInt32(data, offset);
        }

        public void SetIdByIndex(int x, int y, int value)
        {
            int offset = GetIndex(x, y) * cValueOffset;
            int startValue = offset;

            byte[] bytes = BitConverter.GetBytes(value);

            for (int i = 0; i < 4; i++)
            {
                data[startValue + i] = bytes[i];
            }
        }


        public int GetValue(int x, int y)
        {
            int offset = GetIndex(x, y) * (cValueOffset);
            return BitConverter.ToInt32(data, offset + 4);
        }

        public void SetValue(int x, int y, int value)
        {
            int offset = GetIndex(x, y) * (cValueOffset);
            int startValue = offset + 4;

            byte[] bytes = BitConverter.GetBytes(value);

            for (int i = 0; i < 4; i++)
            {
                data[startValue + i] = bytes[i];
            }
        }

        public byte GetByte(int index)
        {
            return data[index];
        }

        public byte[] GetBytes()
        {
            return data;
        }
    }
}