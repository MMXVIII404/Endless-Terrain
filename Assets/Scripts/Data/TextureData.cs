using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;

    // Array of layers for texture settings.
    public Layer[] layers;

    float savedMinHeight;
    float savedMaxHeight;

    // Apply texture settings to a material.
    public void ApplyToMaterial(Material material)
    {
        // Set the number of layers and respective properties in the material.
        material.SetInt("layerCount", layers.Length);
        material.SetColorArray("baseColours", layers.Select(x => x.tint).ToArray());
        material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
        material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
        material.SetFloatArray("baseColourStrength", layers.Select(x => x.tintStrength).ToArray());
        material.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());

        // Generate a texture array and apply it to the material.
        Texture2DArray texturesArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
        material.SetTexture("baseTextures", texturesArray);

        // Update the material's height settings.
        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    // Update mesh heights in the material.
    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        // Save the current min and max height values.
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        // Set the height values in the material.
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

    // Generate a texture array from individual textures.
    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        // Create a new texture array with predefined size and format.
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            // Set the pixels for each texture in the array.
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();
        return textureArray;
    }

    // Inner class to define the properties of a layer.
    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color tint;
        [Range(0, 1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startHeight;
        [Range(0, 1)]
        public float blendStrength;
        public float textureScale;
    }

    // Set various properties of layers.
    public void SetTintStrength(int layer,  float value)
    {
        layers[layer].tintStrength = value;
    }
    public void SetStartHeight(int layer, float value)
    {
        layers[layer].startHeight = value;
    }
    public void SetBlendStrength(int layer, float value)
    {
        layers[layer].blendStrength = value;
    }
    public void SetTextureScale(int layer, float value)
    {
        layers[layer].textureScale = value;
    }
}