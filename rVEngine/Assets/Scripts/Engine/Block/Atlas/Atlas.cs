using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockUtils.Texturing
{
    public class Atlas
    {
        public static Vector2Int PixelScale = new Vector2Int(512, 512);

        public AtlasObject Build(List<Block> blocks)
        {
            Texture2D atlas = Build(blocks, false);
            Texture2D blured = Build(blocks, true);

            return new AtlasObject(atlas, blured);
        }

        private Texture2D Build(List<Block> blocks, bool itsBlur)
        {
            List<Texture2D> textures =new List<Texture2D>();
           
            for (int i = 0; i < blocks.Count; i++)
            {
                Block block = blocks[i];

                if (block.textureType != Block.TextureType.None)
                {
                    Texture2D[] blockTextures = block.GetTextures();

                    if (blockTextures.Length > 1)
                    {
                        Texture2D lastTexture = null;

                        for (int t = 0; t < blockTextures.Length; t++)
                        {
                            Texture2D texture = blockTextures[t];
                            if (texture != lastTexture)
                            {
                                textures.Add(texture);
                                lastTexture = texture;
                            }
                        }
                    }
                    else
                    {
                        Texture2D texture = blockTextures[0];
                        textures.Add(texture);
                    }
                }
            }

            Texture2D[] texureArray = textures.ToArray();
            Texture2D layout = TextureAtlasser.MakeAtlas(ref texureArray, out Rect[] rects, 2, PixelScale, itsBlur);

            layout.filterMode = FilterMode.Point;
            
            if (itsBlur)
            {
                layout.filterMode = FilterMode.Bilinear;
            }

            int rectIndex = 0;

            for (int i = 0; i < blocks.Count; i++)
            {
                Block block = blocks[i];

                if (block.textureType != Block.TextureType.None)
                {
                    Texture2D[] blockTextures = block.GetTextures();

                    if (blockTextures.Length > 1)
                    {
                        Texture2D lastTexture = null;
                        Rect[] textureCoordinates = new Rect[6];

                        for (int t = 0; t < blockTextures.Length; t++)
                        {
                            Texture2D texture = blockTextures[t];
                            if (texture != lastTexture)
                            {
                                textureCoordinates[t] = rects[rectIndex];

                                rectIndex++;
                                lastTexture = texture;
                            }
                            else
                            {
                                textureCoordinates[t] = rects[rectIndex-1];
                            }
                        }

                        block.InitTextureCoordinates(textureCoordinates);
                    }
                    else
                    {
                        Rect[] textureCoordinates = new Rect[1];
                        textureCoordinates[0] = rects[rectIndex];

                        block.InitTextureCoordinates(textureCoordinates);
                        rectIndex++;
                    }
                }
            }
            return layout;
        }
    }
}
