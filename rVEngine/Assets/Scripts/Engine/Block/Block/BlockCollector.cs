using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BlockUtils
{
    public class BlockCollector
    {
        private const string AssetSaverPath = "Block/Saver/Saver";
        public static List<Block> GetExistingBlocks()
        {
            BlockSaver saver = Resources.Load<BlockSaver>(AssetSaverPath);
#if (UNITY_EDITOR)
            if (Application.isEditor)
            {
                saver.Blocks.Clear();

                List<Block> blocks = new List<Block>();
                string[] paths = AssetDatabase.FindAssets("t:Block", null);

                for (int i = 0; i < paths.Length; i++)
                {
                    string path = (AssetDatabase.GUIDToAssetPath(paths[i])).Replace("Assets/Resources/", "").Replace(".asset", "");

                    Block block = Resources.Load<Block>(path);
                    blocks.Add(block);

                    saver.Blocks.Add(block);
                }

                return blocks;
            }
#endif
                return saver.Blocks;
        }
    }
}