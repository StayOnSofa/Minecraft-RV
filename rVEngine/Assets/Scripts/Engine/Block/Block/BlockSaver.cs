using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockUtils {

    [CreateAssetMenu(fileName = "BlockSaver", menuName = "Block/List/BlockSaver")]
    public class BlockSaver : ScriptableObject
    {
        public List<Block> Blocks = new List<Block>();
    }
}
