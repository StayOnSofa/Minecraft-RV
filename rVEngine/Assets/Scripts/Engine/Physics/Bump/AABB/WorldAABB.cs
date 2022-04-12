using BlockUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldUtils;

namespace Bump
{
    public class WorldAABB
    {
        private static List<Box> sBlocks = new List<Box>();
        public static Box[] BoxToWorldKeys(Box box, IWorld world)
        {
            sBlocks.Clear();

            Vector3Int position = Vector3Int.RoundToInt((box.Position + box.Velocity) + box.Scale / 2f);
            Vector3Int size = Vector3Int.CeilToInt(box.Scale);

            for (int x = -(size.x / 2) - 1; x <= (size.x / 2) + 1; x++)
            {
                for (int y = -(size.y / 2) - 1; y <= (size.y / 2) + 1; y++)
                {
                    for (int z = -(size.z / 2) - 1; z <= (size.z / 2) + 1; z++)
                    {
                        Vector3Int blockPosition = position + new Vector3Int(x, y, z);
                        if (world.HasBlock(blockPosition.x, blockPosition.y, blockPosition.z))
                        {
                            int blockId = world.GetBlock(blockPosition.x, blockPosition.y, blockPosition.z);

                            if (!BlockSpecialTypes.IsAirOrSpecial(blockId))
                            {
                                Box _collisionBox = new Box();

                                _collisionBox.Position = blockPosition - (Vector3.one / 2f);
                                _collisionBox.Scale = Vector3.one;

                                sBlocks.Add(_collisionBox);
                            }
                            else 
                            {
                                if (blockId != 0)
                                {
                                    BlockHandler blockHandler = world.GetBlockHandler();
                                    Block block = blockHandler.GetBlock(blockId);

                                    Box _collisionBox = new Box();

                                    _collisionBox.Position = blockPosition;
                                    _collisionBox.Position += block.PointerPosition;

                                    _collisionBox.Scale = block.PointerSize;
                                    _collisionBox.Position += -(block.PointerSize / 2f);


                                    sBlocks.Add(_collisionBox);
                                }
                            }
                        }
                    }
                }
            }

            return sBlocks.ToArray();
        }
    }
}