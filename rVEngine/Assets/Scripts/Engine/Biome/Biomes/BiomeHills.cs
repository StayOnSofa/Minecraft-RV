using BiomeUtils.NoiseUtils;
using UnityEngine;

namespace BiomeUtils
{
    public class BiomeHills : Biome
    {
        private NoiseMapGenerator _noiseMapGenerator;
        public override void SetDefaults()
        {
            Border = 0.3f;
            Lerp = 0.25f;
            BiomeFrequency = 1f;
            HeightFrequency = 1.3f;
            GroundJump = 8;

            _noiseMapGenerator = new NoiseMapGenerator(Seed);
        }

        public override int GetBlock(int x, int height, int z, int topHeight)
        {
            if (topHeight == height)
            {
                bool place = _noiseMapGenerator.GetObjectSpawn(x, z, 64);
                bool poppy = _noiseMapGenerator.GetObjectSpawn(x + 612, z + 212, 8);

                if (place)
                {
                    return BlockRegister.StructureTree;
                }

                if (poppy)
                {
                    return BlockRegister.Sphere;
                }
            }
            else
            {
                int blockId = BlockRegister.Grass;
                if (height < (topHeight - 6))
                {
                    blockId = BlockRegister.Stone;
                }

                return blockId;
            }

            return BlockRegister.Air;
        }
    }
}
