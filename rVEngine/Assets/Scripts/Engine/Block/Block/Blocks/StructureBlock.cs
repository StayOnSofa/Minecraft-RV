using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace BlockUtils
{
    public abstract class StructureBlock : StandartBlock
    {
        public override void BreakBlock(IWorld world, int x, int y, int z)
        {
            return;
        }

        protected abstract void BuildStructure(IWorld world, int x, int y, int z);

        public override void PlaceBlock(IWorld world, int x, int y, int z)
        {
            BuildStructure(world, x, y, z);
        }
    }
}
