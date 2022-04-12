using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ChunkUtils.FileSystem
{
    public class RegionFileTesting : MonoBehaviour
    {
        private void Start()
        {
            new Thread(() =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                RegionFile file = new RegionFile(0, 0);
                RegionRegister register = file.GetRegionRegister();

                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        Debug.Log(register.GetValue(x, y));
                    }
                }

                stopWatch.Stop();
                Debug.Log("Time: " + stopWatch.ElapsedMilliseconds / 1000f);

                file.Close();
            }).Start();
        }

    }
}