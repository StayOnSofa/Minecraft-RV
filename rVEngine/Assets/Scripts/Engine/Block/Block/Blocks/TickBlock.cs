using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace BlockUtils
{
    [CreateAssetMenu(fileName = "TickBlock", menuName = "Block/TickBlock")]
    public class TickBlock : Block
    {
        public override void BreakBlock(IWorld world, int x, int y, int z)
        {
            return;
        }

        public override void PlaceBlock(IWorld world, int x, int y, int z)
        {
            return;
        }

        public virtual void Tick(IWorld world, Vector3Int position, int tick)
        {
           
        }
    }
}