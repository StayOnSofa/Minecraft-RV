using BlockUtils;
using BlockUtils.Texturing;
using ChunkUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Render.MeshUtils
{
    public class MeshChunk
    {
        private static MeshBlockSide _xPlus;
        private static MeshBlockSide _xMinus;
        private static MeshBlockSide _yPlus;
        private static MeshBlockSide _yMinus;
        private static MeshBlockSide _zPlus;
        private static MeshBlockSide _zMinus;

        private static bool _initialized = false;
        private static void Init()
        {
            if (!_initialized)
            {
                _xPlus = BlockSidePlusX.GetMesh();
                _xMinus = BlockSideMinusX.GetMesh();

                _yPlus = BlockSidePlusY.GetMesh();
                _yMinus = BlockSideMinusY.GetMesh();

                _zPlus = BlockSidePlusZ.GetMesh();
                _zMinus = BlockSideMinusZ.GetMesh();

                _initialized = true;
            }
        }

        private IChunk _chunk;
        private BlockHandler _blockHandler;

        private ExpandableMesh _meshWithCollider;
        private ExpandableMesh _meshWithoutCollider;

        private int _regularBlockWithColliderBorder;
        private int _specialBlockWithColliderBorder;

        public MeshChunk(IChunk chunk)
        {
            Init();

            _meshWithCollider = new ExpandableMesh();
            _meshWithoutCollider = new ExpandableMesh();

            _chunk = chunk;
            _blockHandler = _chunk.GetBlockHandler();
        }

        public void BuildMesh(int layer)
        {
            Clear();

            for (int x = 0; x < PhysicalChunk.Slice.x; x++)
            {
                int sLayer = layer * (int)PhysicalChunk.Slice.y;
                int eLayer = sLayer + (int)PhysicalChunk.Slice.y;

                for (int y = sLayer; y < eLayer; y++)
                {
                    int _y = y - sLayer;

                    for (int z = 0; z < PhysicalChunk.Slice.z; z++)
                    {
                        int blockId = _chunk.GetBlock(x, y, z);

                        if (blockId != BlockRegister.Air)
                        {
                            BlockUtils.Block block = _blockHandler.GetBlock(blockId);

                            ExpandableMesh expandable = _meshWithoutCollider;

                            if (IsCollide(blockId))
                            {
                                expandable = _meshWithCollider;
                            }

                            if (!IsAirOrSpecial(blockId))
                            {
                                int _plusX = x + 1;
                                if (IsAirOrSpecial(_plusX, y, z))
                                {
                                    AddSide(expandable, block, new Vector3(x, _y, z), BlockSide.X_PLUS);
                                }

                                int _minusX = x - 1;
                                if (IsAirOrSpecial(_minusX, y, z))
                                {
                                    AddSide(expandable, block, new Vector3(x, _y, z), BlockSide.X_MINUS);
                                }

                                int _plusZ = z + 1;
                                if (IsAirOrSpecial(x, y, _plusZ))
                                {
                                    AddSide(expandable, block, new Vector3(x, _y, z), BlockSide.Z_PLUS);
                                }

                                int _minusZ = z - 1;
                                if (IsAirOrSpecial(x, y, _minusZ))
                                {
                                    AddSide(expandable, block, new Vector3(x, _y, z), BlockSide.Z_MINUS);
                                }

                                int _plusY = y + 1;
                                if (IsAirOrSpecial(x, _plusY, z))
                                {
                                    AddSide(expandable, block, new Vector3(x, _y, z), BlockSide.Y_PLUS);
                                }

                                int _minusY = y - 1;
                                if (IsAirOrSpecial(x, _minusY, z))
                                {
                                    AddSide(expandable, block, new Vector3(x, _y, z), BlockSide.Y_MINUS);
                                }
                            }
                            else
                            {
                                AddModel(expandable, block, new Vector3(x, _y, z));
                            }
                        }
                    }
                }
            }

            ApplyMesh();
        }

        private bool IsAirOrSpecial(int x, int y, int z)
        {
            int blockId = _chunk.GetBlock(x, y, z);

            return BlockSpecialTypes.IsAirOrSpecial(blockId);
        }

        private bool IsAirOrSpecial(int blockId)
        {
            return BlockSpecialTypes.IsAirOrSpecial(blockId);
        }

        private bool IsCollide(int blockId)
        {
            return BlockSpecialTypes.IsCollide(blockId);
        }

        private void Clear()
        {
            _meshWithCollider.Clear();
            _meshWithoutCollider.Clear();
        }

        private void ApplyMesh()
        {
            _meshWithCollider.Apply();
            _meshWithoutCollider.Apply();
        }

        private void AddSide(ExpandableMesh expandable, BlockUtils.Block block, Vector3 position, BlockSide side)
        {
            Rect textureCoordinates = block.GetTextureCoordinate(side);

            switch (side)
            {
                case BlockSide.X_MINUS:
                    expandable.AddSide(textureCoordinates, position, _xMinus.Vertices, _xMinus.Uvs, _xMinus.Normal, _xMinus.Triangles);
                    break;
                case BlockSide.X_PLUS:
                    expandable.AddSide(textureCoordinates, position, _xPlus.Vertices, _xPlus.Uvs, _xPlus.Normal, _xPlus.Triangles);
                    break;
                case BlockSide.Z_PLUS:
                    expandable.AddSide(textureCoordinates, position, _yMinus.Vertices, _yMinus.Uvs, _yMinus.Normal, _yMinus.Triangles);
                    break;
                case BlockSide.Z_MINUS:
                    expandable.AddSide(textureCoordinates, position, _yPlus.Vertices, _yPlus.Uvs, _yPlus.Normal, _yPlus.Triangles);
                    break;
                case BlockSide.Y_MINUS:
                    expandable.AddSide(textureCoordinates, position, _zMinus.Vertices, _zMinus.Uvs, _zMinus.Normal, _zMinus.Triangles);
                    break;
                case BlockSide.Y_PLUS:
                    expandable.AddSide(textureCoordinates, position, _zPlus.Vertices, _zPlus.Uvs, _zPlus.Normal, _zPlus.Triangles);
                    break;
            }
        }

        private void AddModel(ExpandableMesh expandable, BlockUtils.Block block, Vector3 position)
        {
            expandable.AddMesh(block.CustomMesh, block.GetTextureCoordinate(), position);
        }

        public Mesh GetMeshWithCollider()
        {
            return _meshWithCollider.GetMesh();
        }

        public Mesh GetMeshWithoutCollider()
        {
            return _meshWithoutCollider.GetMesh();
        }
    }
}
