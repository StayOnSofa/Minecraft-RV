using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace BlockUtils
{
    [CreateAssetMenu(fileName = "StructureGeode", menuName = "Block/StructureGeode")]
    public class StructureGeode : StructureBlock
    {
        protected override void BuildStructure(IWorld world, int x, int y, int z)
        {
            world.BreackBlock(x, y, z);
            int random = ((y + x + z) % 10);

            int height = 7;
            if (random == 0)
            {
                height += 1;
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
                            world.PlaceBlock(x + _x, y + _y, z + _z, BlockRegister.Granite);
                        }
                    }
                }
            }

        }
    }
}
