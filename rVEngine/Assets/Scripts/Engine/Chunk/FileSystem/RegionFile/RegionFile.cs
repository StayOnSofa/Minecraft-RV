using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ChunkUtils.FileSystem
{
    public class RegionFile
    {
        private string filePath = $"D:/DefaultWorld/regions/";
        private const int RegisterBytes = RegionRegister.cRegisterBytes;

        private FileStream _fileStream;

        private string _basePath;
        private RegionRegister _regionRegister;

        public RegionFile(int x, int y)
        {
            _basePath = filePath;
            _fileStream = GetOrCreate(filePath + $"{x}{y}.dcf");
        }

        public void WirteRegister(FileStream fileStream, RegionRegister register)
        {
            fileStream.Write(register.GetBytes(), 0, RegisterBytes);
        }
        public void WirteRegister(RegionRegister register)
        {
            WirteRegister(_fileStream, register);
        }

        private FileStream GetOrCreate(string path)
        {
            FileStream fileStream;

            if (File.Exists(path))
            {
                fileStream = File.Open(path, FileMode.Open);
                _regionRegister = ReadRegister(fileStream);
            }
            else
            {
                Directory.CreateDirectory(_basePath);

                RegionRegister regionRegister = new RegionRegister();
                fileStream = File.Create(path);

                WirteRegister(fileStream, regionRegister);
                _regionRegister = regionRegister;
            }

            return fileStream;
        }

        private RegionRegister ReadRegister()
        {
            byte[] bytes = new byte[RegisterBytes];
            int numBytesToRead = (int)RegisterBytes;
            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                int n = _fileStream.Read(bytes, numBytesRead, numBytesToRead);

                if (n == 0)
                    break;

                numBytesRead += n;
                numBytesToRead -= n;
            }


            RegionRegister regionRegister = new RegionRegister(bytes);
            return regionRegister;
        }

        private RegionRegister ReadRegister(FileStream stream)
        {
            byte[] bytes = new byte[RegisterBytes];
            int numBytesToRead = (int)RegisterBytes;
            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                int n = stream.Read(bytes, numBytesRead, numBytesToRead);

                if (n == 0)
                    break;

                numBytesRead += n;
                numBytesToRead -= n;
            }


            RegionRegister regionRegister = new RegionRegister(bytes);
            return regionRegister;
        }

        public RegionRegister GetRegionRegister()
        {
            return _regionRegister;
        }

        public void Close()
        {
            _fileStream.Close();
            _fileStream.Dispose();
        }

    }
}