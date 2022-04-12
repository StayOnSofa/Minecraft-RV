using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNoiseOnPlane : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MeshRenderer _meshRenderer1;

    private void BuildCeils()
    {
        FastNoiseLite noise = new FastNoiseLite();

        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        Vector2 minMax = Vector3.zero;

        var texture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;


        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                float value = noise.GetNoise(x, y);
                float lValue = (value + 1f) / 2f;

                if (lValue < 0.3)
                {
                    lValue = 0;
                }

                int biome = Mathf.RoundToInt((lValue + 0.3f) * 7f);

                minMax = CalculateMinimanAndMaximalValues(Mathf.Round(biome/7f * 512));

                texture.SetPixel(x, y, new Color(biome / 7f, biome / 7f, biome / 7f));

            }
        }

        Debug.Log(minMax);

        texture.Apply();
        _meshRenderer.material.mainTexture = texture;
    }

    private void Start()
    {
        BuildCeils();

        FastNoiseLite noise = new FastNoiseLite();

        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        Vector2 minMax = Vector3.zero;

        var texture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;


        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                float value = noise.GetNoise(x, y);
                float lValue = (value + 1f) / 2f;

                float lerp = lValue;

                lerp += 0.3f;

                if (lerp > 0.1 && lerp < 0.2)
                {
                    lerp = 0;
                }

                texture.SetPixel(x, y, new Color(lerp, lerp, lerp));

            }
        }

        texture.Apply();

        _meshRenderer1.material.mainTexture = texture;
    }

    private float minimal = 1;
    private float maximal = 0;

    private Vector2 CalculateMinimanAndMaximalValues(float value)
    {
        if (minimal > value)
        {
            minimal = value;
        }

        if (maximal < value)
        {
            maximal = value;
        }

        return new Vector2(minimal, maximal);
    }

}
