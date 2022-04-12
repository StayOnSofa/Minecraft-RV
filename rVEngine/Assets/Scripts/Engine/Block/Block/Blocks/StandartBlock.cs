using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace BlockUtils
{
    [CreateAssetMenu(fileName = "StandartBlock", menuName = "Block/StandartBlock")]
    public class StandartBlock : Block
    {
        public override void BreakBlock(IWorld world, int x, int y, int z)
        {
            return;
        }

        public override void PlaceBlock(IWorld world, int x, int y, int z)
        {
            return;
        }
    }
}
