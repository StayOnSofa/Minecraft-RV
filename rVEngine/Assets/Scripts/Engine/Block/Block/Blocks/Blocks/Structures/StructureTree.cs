using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace BlockUtils
{
    [CreateAssetMenu(fileName = "StructureTree", menuName = "Block/StructureTree")]
    public class StructureTree : StructureBlock
    {
        protected override void BuildStructure(IWorld world, int x, int y, int z)
        {
            world.BreackBlock(x, y, z);
            int random = ((y + x + z) % 10);

            int height = 6;
            if (random == 0)
            {
                height += 3;
            }


            for (int i = 0; i < height - 1; i++)
            {
                world.BreackBlock(x, y, z);
                world.PlaceBlock(x, y + i, z, BlockRegister.Log);
            }

            int r = height / 2;

            for (int _x = -r; _x < r; _x++)
            {
                for (int _y = -r; _y < r; _y++)
                {
                    for (int _z = -r; _z < r; _z++)
                    {
                        float distance = Vector3.Distance(new Vector3(_x, _y, _z), Vector3.zero);
                        if (distance < r)
                        {
                            world.PlaceBlock(x + _x, y + _y + (height - 2), z + _z, BlockRegister.Leaves);
                        }
                    }
                }
            }

            world.PlaceBlock(x, y, z, BlockRegister.Log);
        }
    }
}
