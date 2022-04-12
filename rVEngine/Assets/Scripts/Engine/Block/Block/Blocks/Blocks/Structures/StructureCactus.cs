using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace BlockUtils
{
    [CreateAssetMenu(fileName = "StructureCactus", menuName = "Block/StructureCactus")]
    public class StructureCactus : StructureBlock
    {
        protected override void BuildStructure(IWorld world, int x, int y, int z)
        {
            world.BreackBlock(x, y, z);

            int random = ((y + x + z) % 10);

            int height = 4;
            if (random == 0)
            {
                height += 1;
            }

            for (int i = 1; i < height; i++)
            {
                world.PlaceBlock(x, y+i, z, BlockRegister.Cactus);
            }

            world.PlaceBlock(x, y, z, BlockRegister.Sand);
        }
    }
}