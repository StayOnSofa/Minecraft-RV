using BiomeUtils.NoiseUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BiomeUtils
{
    public class BiomeMountain : Biome
    {
        private NoiseMapGenerator _noiseMapGenerator;
        public override void SetDefaults()
        {
            Border = 0.3f;
            Lerp = 0.05f;
            BiomeFrequency = 2f;
            HeightFrequency = 1.2f;
            GroundJump = 50;

            _noiseMapGenerator = new NoiseMapGenerator(Seed);
        }

        public override void InitNoiseGenerator(int seed)
        {
            Seed = seed;

            NoiseGenerator = new FastNoiseLite(seed);
            NoiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            NoiseGenerator.SetFractalType(FastNoiseLite.FractalType.PingPong);
        }

        public override int GetBlock(int x, int height, int z, int topHeight)
        {
            if (topHeight - 2 == height)
            {
                bool place = _noiseMapGenerator.GetObjectSpawn(x, z, 2048);
                if (place)
                {
                    return BlockRegister.StructureGeode;
                }
            }

            return BlockRegister.Stone;
        }
    }
}
