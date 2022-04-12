using BlockUtils.Texturing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockUtils
{
    public class BlockHandler
    {
        public static int RegularBlockWithColliderBorder;
        public static int SpecialBlockWithColliderBorder;

        private Block[] _regularBlockInstances;
        private Block[] _specialBlockInstances;

        private Dictionary<string, Block> _registerBlockValues = new Dictionary<string, Block>();

        private AtlasObject _atlasObject;

        public BlockHandler()
        {
            Register();
        }

        private void Register()
        {
            CompileList();

            Atlas atlas = new Atlas();

            List<Block> _prepareAtlasBlocks = new List<Block>();

            foreach (Block block in _regularBlockInstances)
            {
                _prepareAtlasBlocks.Add(block);
            }

            foreach (Block block in _specialBlockInstances)
            {
                _prepareAtlasBlocks.Add(block);
            }

            _atlasObject = atlas.Build(_prepareAtlasBlocks);
        }

        private int CalculateRegularBlockSize(List<Block> oBlocks)
        {
            int regulaBlockCount = 0;

            for (int i = 0; i < oBlocks.Count; i++)
            {
                Block block = oBlocks[i];

                if (!block.IsHasCustomMesh)
                {
                    regulaBlockCount++;
                }
            }

            return regulaBlockCount;
        }

        private int CalculateSpecialBlockSize(List<Block> oBlocks)
        {
            int specialBlockCount = 0;

            for (int i = 0; i < oBlocks.Count; i++)
            {
                Block block = oBlocks[i];

                if (block.IsHasCustomMesh)
                {
                    specialBlockCount++;
                }
            }

            return specialBlockCount;
        }

        private void RegularBlockFill(List<Block> oBlocks)
        {
            int regularBlockCount = CalculateRegularBlockSize(oBlocks);
            _regularBlockInstances = new Block[regularBlockCount];

            int regularIndex = 0;
            int regularLocalIndex = 0;
            int regularCollidesBorder = 0;

            for (int i = 0; i < oBlocks.Count; i++)
            {
                Block block = oBlocks[i];

                if (!block.IsHasCustomMesh)
                {
                    if (block.IsCollided)
                    {
                        _regularBlockInstances[regularLocalIndex] = block;
                        block.Init(regularIndex);

                        regularIndex++;
                        regularLocalIndex++;
                        regularCollidesBorder++;
                    }
                }
            }

            for (int i = 0; i < oBlocks.Count; i++)
            {
                Block block = oBlocks[i];

                if (!block.IsHasCustomMesh)
                {
                    if (!block.IsCollided)
                    {
                        _regularBlockInstances[regularLocalIndex] = block;
                        block.Init(regularIndex);

                        regularIndex++;
                        regularLocalIndex++;
                    }
                }
            }

            RegularBlockWithColliderBorder = regularCollidesBorder;
        }

        private void SpecialBlockFill(List<Block> oBlocks)
        {
            int specialBlockCount = CalculateSpecialBlockSize(oBlocks);
            _specialBlockInstances = new Block[specialBlockCount];

            int specialIndex = 1;
            int specialLocalIndex = 0;
            int specialCollidesBorder = 1;

            for (int i = 0; i < oBlocks.Count; i++)
            {
                Block block = oBlocks[i];

                if (block.IsHasCustomMesh)
                {
                    if (block.IsCollided)
                    {
                        _specialBlockInstances[specialLocalIndex] = block;
                        block.Init(-specialIndex);

                        specialIndex++;
                        specialLocalIndex++;
                        specialCollidesBorder++;
                    }
                }
            }

            for (int i = 0; i < oBlocks.Count; i++)
            {
                Block block = oBlocks[i];

                if (block.IsHasCustomMesh)
                {
                    if (!block.IsCollided)
                    {
                        _specialBlockInstances[specialLocalIndex] = block;
                        block.Init(-specialIndex);

                        specialIndex++;
                        specialLocalIndex++;
                    }
                }
            }

            SpecialBlockWithColliderBorder = -specialCollidesBorder;
        }

        private void CompileList()
        {
            List<Block> oBlocks = BlockCollector.GetExistingBlocks();

            RegularBlockFill(oBlocks);
            SpecialBlockFill(oBlocks);

            for (int i = 0; i < oBlocks.Count; i++)
            {
                Block block = oBlocks[i];
                _registerBlockValues.Add(block.RegisterTitle, block);
            }

            Debug.Log("RegularBorder: " +RegularBlockWithColliderBorder);
            Debug.Log("SpecialBorder: " +SpecialBlockWithColliderBorder);
        }

        public Block GetBlock(int id)
        {
            if (id >= 0)
            {
                return _regularBlockInstances[id];
            }

            int specialId = -(id + 1);

            return _specialBlockInstances[specialId];
        }

        public Block GetBlock(string registerTitle)
        {
            return _registerBlockValues[registerTitle];
        }


        public AtlasObject GetAtlasObject()
        {
            return _atlasObject;
        }
    }
}
