using System;
using System.Collections;
using UnityEngine;

public static class HeightMapGenerator
{
    public static IEnumerator GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre, Action<HeightMap> callback)
    {
        // delay
        yield return new WaitForSeconds(0.1f);

        // Generate a noise map with the given parameters.
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);
        AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        // Iterate through each value in the noise map.
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Apply the height curve and multiplier to each value.
                values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;
                if (values[i, j] > maxValue) maxValue = values[i, j];
                if (values[i, j] < minValue) minValue = values[i, j];
            }
        }

        // Create a HeightMap object with the processed values.
        HeightMap heightMap = new HeightMap(values, minValue, maxValue);
        callback(heightMap);
    }
}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}