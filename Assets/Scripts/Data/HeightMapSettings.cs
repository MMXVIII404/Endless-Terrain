using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Noise;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{

    public NoiseSettings noiseSettings;

    public bool useFalloff;

    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();
        base.OnValidate();
    }
#endif

    public void SetNormalizeMode(int mode)
    {
        switch (mode)
        {
            case 0:
                noiseSettings.normalizeMode = NormalizeMode.Local; break;
            case 1:
                noiseSettings.normalizeMode = NormalizeMode.Global; break;
        }
        noiseSettings.ValidateValues();
    }

    public void SetFalloff(int falloff)
    {
        switch (falloff)
        {
            case 0:
                useFalloff = true; break;
            case 1:
                useFalloff = false; break;
        }
        noiseSettings.ValidateValues();
    }

    public void SetNoiseScale(float scale)
    {
        noiseSettings.scale = scale;
        noiseSettings.ValidateValues();
    }

    public void SetOctaves(int octaves)
    {
        noiseSettings.octaves = octaves;
        noiseSettings.ValidateValues();
    }
    public void SetPersistance(float persistance)
    {
        noiseSettings.persistance = persistance;
        noiseSettings.ValidateValues();
    }
    public void SetLacunarity(float lacunarity)
    {
        noiseSettings.lacunarity = lacunarity;
        noiseSettings.ValidateValues();
    }
    public void SetSeed(int seed)
    {
        noiseSettings.seed = seed;
        noiseSettings.ValidateValues();
    }
    public void SetOffsetX(float offsetX)
    {
        noiseSettings.offset.x = offsetX;
        noiseSettings.ValidateValues();
    }
    public void SetOffsetY(float offsetY)
    {
        noiseSettings.offset.y = offsetY;
        noiseSettings.ValidateValues();
    }

    public void SetHeightMultiplier(float heightMultiplier)
    {
        this.heightMultiplier = heightMultiplier;
        noiseSettings.ValidateValues();
    }
}