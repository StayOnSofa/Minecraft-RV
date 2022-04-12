using BiomeUtils.NoiseUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BiomeUtils
{
    public class BiomeDesert : Biome
    {
        private NoiseMapGenerator _noiseMapGenerator;
        public override void SetDefaults()
        {
            Border = 0.4f;
            Lerp = 0.05f;
            BiomeFrequency = 8f;
            HeightFrequency = 1.1f;
            GroundJump = 3;

            _noiseMapGenerator = new NoiseMapGenerator(Seed);
        }

        public override int GetBlock(int x, int height, int z, int topHeight)
        {
            if (topHeight == height)
            {
                bool place = _noiseMapGenerator.GetObjectSpawn(x, z, 512);
                if (place)
                {
                    return BlockRegister.StructureCactus;
                }
            }

            return BlockRegister.Sand;
        }
    }
}
