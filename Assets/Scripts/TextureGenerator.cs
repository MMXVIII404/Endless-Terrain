using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    // Create a Texture2D from a color map.
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        // Initialize a new Texture2D object with given dimensions.
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    // Create a Texture2D from a height map.
    public static Texture2D TextureFromHeightMap(HeightMap heightMap)
    {
        // Get dimensions from the height map.
        int width = heightMap.values.GetLength(0);
        int height = heightMap.values.GetLength(1);

        // Initialize a color map based on the height values.
        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Map the height values to colors (black to white).
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]));
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }

}