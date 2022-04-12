using MyBox;
using Render.MeshUtils;
using UnityEngine;
using WorldUtils;

namespace BlockUtils
{
    public abstract class Block : ScriptableObject
    {
        [Separator("Register")]
        [SerializeField] public string RegisterTitle;
        [SerializeField] public string Title;

        public enum TextureType
        { 
            None,
            Single,
            Separated
        }

        [Separator("Block Render State")]
        public TextureType textureType = TextureType.Single;
        [ConditionalField(nameof(textureType), true, TextureType.None)] public bool IsHasCustomMesh = false;

        [ConditionalField(nameof(textureType), false, TextureType.Single)] public Texture2D MainTexture;

        [ConditionalField(nameof(textureType), false, TextureType.Separated)] public Texture2D TopTexture;
        [ConditionalField(nameof(textureType), false, TextureType.Separated)] public Texture2D DownTexture;
        [ConditionalField(nameof(textureType), false, TextureType.Separated)] public Texture2D LeftTexture;
        [ConditionalField(nameof(textureType), false, TextureType.Separated)] public Texture2D RightTexture;
        [ConditionalField(nameof(textureType), false, TextureType.Separated)] public Texture2D BackTexture;
        [ConditionalField(nameof(textureType), false, TextureType.Separated)] public Texture2D ForwardTexture;

        [SerializeField] public bool IsCollided = true;

        [SerializeField] public Vector3 PointerSize = Vector3.one * 1.025f;
        [SerializeField] public Vector3 PointerPosition = Vector3.zero;

        [Separator("Models")]
        [ConditionalField(nameof(IsHasCustomMesh))] public Mesh CustomMesh;


        private Rect[] _textureCoordinates;

        private int _id;

        private bool _flagInitCoordinates = false;
        private bool _flagInit = false;

        private bool _flagIsSingleTexture = false;

        public void InitTextureCoordinates(Rect[] rects)
        {
            if (!_flagInitCoordinates)
            {
                _textureCoordinates = rects;

                if (_textureCoordinates.Length == 1)
                {
                    _flagIsSingleTexture = true;
                }

                _flagInitCoordinates = true;
            }
        }

        public virtual void Init(int id)
        {
            if (!_flagInit)
            {
                _id = id;

                _flagInit = true;
            }
        }

        public int GetBlockId()
        {
            return _id;
        }

        public Rect GetTextureCoordinate(BlockSide side)
        {
            if (_flagIsSingleTexture)
            {
                return _textureCoordinates[0];
            }
            else {
                int id = (int)side;

                return _textureCoordinates[id];
            }
        }

        public Rect GetTextureCoordinate()
        {
            return _textureCoordinates[0];
        }

        public Texture2D[] GetTextures()
        {
            if (textureType == TextureType.Single)
            {
                Texture2D[] textures = new Texture2D[1];
                textures[0] = MainTexture;

                return textures;
            }
            else {
                Texture2D[] textures = new Texture2D[6];
                
                textures[0] = TopTexture;
                textures[1] = DownTexture;
                textures[2] = LeftTexture;
                textures[3] = RightTexture;
                textures[4] = BackTexture;
                textures[5] = ForwardTexture;

                return textures;
            }
        }

        public Mesh GetCustomMesh()
        {
            return CustomMesh;
        }

        public abstract void PlaceBlock(IWorld world, int x, int y, int z);
        public abstract void BreakBlock(IWorld world, int x, int y, int z);
    }
}