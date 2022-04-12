using BlockUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockUtils
{
    public static class BlockSpecialTypes
    {
        public static bool IsAirOrSpecial(int blockId)
        {
            if (blockId < 0 || blockId == BlockRegister.Air)
            {
                return true;
            }

            return false;
        }

        public static bool IsCollide(int blockId)
        {
            if (blockId > BlockHandler.SpecialBlockWithColliderBorder && blockId < BlockHandler.RegularBlockWithColliderBorder)
            {
                return true;
            }

            return false;
        }
    }
}