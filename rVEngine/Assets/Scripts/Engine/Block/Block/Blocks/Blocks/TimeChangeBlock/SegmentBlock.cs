using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace BlockUtils
{
    [CreateAssetMenu(fileName = "SegmentBlock", menuName = "Block/Time/Segment")]
    public class SegmentBlock : TickBlock
    {
        private static System.Random _random = new System.Random();
        protected static int GetProcentRandom()
        {
            return _random.Next() * 100;
        }

        private float _procentGrow = 0;
        private Block _blockToChange;

        public void PostInit(float procentGrow, Block blockToChange)
        {
            _procentGrow = procentGrow;
            _blockToChange = blockToChange;
        }

        public override void BreakBlock(IWorld world, int x, int y, int z)
        {
            return;
        }

        public override void PlaceBlock(IWorld world, int x, int y, int z)
        {
            return;
        }

        public override void Tick(IWorld world, Vector3Int position, int tick)
        {
            if (GetProcentRandom() >= _procentGrow) { 

                world.BreackBlock(position.x, position.y, position.z);
                world.PlaceBlock(position.x, position.y, position.z, _blockToChange.GetBlockId());

            }
        }
    }
}
