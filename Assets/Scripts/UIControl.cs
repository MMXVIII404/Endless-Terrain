using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIControl : MonoBehaviour
{
    [SerializeField]
    TMP_Dropdown mapPreviewDropDown;
    [SerializeField]
    TMP_Dropdown normalizeModeDropDown;
    [SerializeField]
    TMP_Dropdown fallOffDropDown;
    [SerializeField]
    TMP_Dropdown flatShadingDropDown;
    [SerializeField]
    Slider lodSlider;
    [SerializeField]
    Slider noiseScaleSlider;
    [SerializeField]
    Slider octavesSlider;
    [SerializeField]
    Slider persistanceSlider;
    [SerializeField]
    Slider lacunaritySlider;
    [SerializeField]
    Slider seedSlider;
    [SerializeField]
    Slider offsetXSlider;
    [SerializeField]
    Slider offsetYSlider;
    [SerializeField]
    Slider heightMultiplierSlider;

    [SerializeField]
    Slider waterTintSlider;
    [SerializeField]
    Slider waterStartHeightSlider;
    [SerializeField]
    Slider waterBlendSlider;
    [SerializeField]
    Slider waterTextureScaleSlider;

    [SerializeField]
    Slider sandyGrassTintSlider;
    [SerializeField]
    Slider sandyGrassStartHeightSlider;
    [SerializeField]
    Slider sandyGrassBlendSlider;
    [SerializeField]
    Slider sandyGrassTextureScaleSlider;

    [SerializeField]
    Slider grassTintSlider;
    [SerializeField]
    Slider grassStartHeightSlider;
    [SerializeField]
    Slider grassBlendSlider;
    [SerializeField]
    Slider grassTextureScaleSlider;

    [SerializeField]
    Slider stonyGroundTintSlider;
    [SerializeField]
    Slider stonyGroundStartHeightSlider;
    [SerializeField]
    Slider stonyGroundBlendSlider;
    [SerializeField]
    Slider stonyGroundTextureScaleSlider;

    [SerializeField]
    Slider rockTintSlider;
    [SerializeField]
    Slider rockStartHeightSlider;
    [SerializeField]
    Slider rockBlendSlider;
    [SerializeField]
    Slider rockTextureScaleSlider;

    [SerializeField]
    Slider snowTintSlider;
    [SerializeField]
    Slider snowStartHeightSlider;
    [SerializeField]
    Slider snowBlendSlider;
    [SerializeField]
    Slider snowTextureScaleSlider;

    [SerializeField]
    TextureData textureData;
    [SerializeField]
    MapPreview mapPreview;
    [SerializeField]
    HeightMapSettings heightMapSettings;
    [SerializeField]
    MeshSettings meshSettings;

    private void Start()
    {
        mapPreview.SetDrawMode(mapPreviewDropDown.value);
        mapPreviewDropDown.onValueChanged.AddListener(MapPreviewChange);

        heightMapSettings.SetNormalizeMode(normalizeModeDropDown.value);
        normalizeModeDropDown.onValueChanged.AddListener(NormalizeModeChange);

        heightMapSettings.SetFalloff(fallOffDropDown.value);
        fallOffDropDown.onValueChanged.AddListener(FalloffChange);

        meshSettings.SetFlatShading(flatShadingDropDown.value);
        flatShadingDropDown.onValueChanged.AddListener(FlatShadingChange);

        mapPreview.editorPreviewLOD = 0;
        lodSlider.onValueChanged.AddListener(LODChange);

        heightMapSettings.SetNoiseScale(noiseScaleSlider.value);
        noiseScaleSlider.onValueChanged.AddListener(NoiseScaleChange);
        heightMapSettings.SetOctaves((int)octavesSlider.value);
        octavesSlider.onValueChanged.AddListener(OctavesChange);

        heightMapSettings.SetPersistance(persistanceSlider.value);
        persistanceSlider.onValueChanged.AddListener(PersistanceChange);

        heightMapSettings.SetLacunarity(lacunaritySlider.value);
        lacunaritySlider.onValueChanged.AddListener(LacunarityChange);

        heightMapSettings.SetSeed((int)seedSlider.value);
        seedSlider.onValueChanged.AddListener(SeedChange);

        heightMapSettings.SetOffsetX(offsetXSlider.value);
        offsetXSlider.onValueChanged.AddListener(OffsetXChange);

        heightMapSettings.SetOffsetY(offsetYSlider.value);
        offsetYSlider.onValueChanged.AddListener(OffsetYChange);

        heightMapSettings.SetHeightMultiplier(heightMultiplierSlider.value);
        heightMultiplierSlider.onValueChanged.AddListener(HeightMultiplierChange);

        textureData.SetTintStrength(0, waterTintSlider.value);
        textureData.SetTintStrength(1, sandyGrassTintSlider.value);
        textureData.SetTintStrength(2, grassTintSlider.value);
        textureData.SetTintStrength(3, stonyGroundTintSlider.value);
        textureData.SetTintStrength(4, rockTintSlider.value);
        textureData.SetTintStrength(5, snowTintSlider.value);
        waterTintSlider.onValueChanged.AddListener(value => SetTintStrength("water", value));
        sandyGrassTintSlider.onValueChanged.AddListener(value => SetTintStrength("Sandy Grass", value));
        grassTintSlider.onValueChanged.AddListener(value => SetTintStrength("Grass", value));
        stonyGroundTintSlider.onValueChanged.AddListener(value => SetTintStrength("Stony Ground", value));
        rockTintSlider.onValueChanged.AddListener(value => SetTintStrength("Rock", value));
        snowTintSlider.onValueChanged.AddListener(value => SetTintStrength("Snow", value));

        textureData.SetStartHeight(0, waterStartHeightSlider.value);
        textureData.SetStartHeight(1, sandyGrassStartHeightSlider.value);
        textureData.SetStartHeight(2, grassStartHeightSlider.value);
        textureData.SetStartHeight(3, stonyGroundStartHeightSlider.value);
        textureData.SetStartHeight(4, rockStartHeightSlider.value);
        textureData.SetStartHeight(5, snowStartHeightSlider.value);
        waterStartHeightSlider.onValueChanged.AddListener(value => SetStartHeight("water", value));
        sandyGrassStartHeightSlider.onValueChanged.AddListener(value => SetStartHeight("Sandy Grass", value));
        grassStartHeightSlider.onValueChanged.AddListener(value => SetStartHeight("Grass", value));
        stonyGroundStartHeightSlider.onValueChanged.AddListener(value => SetStartHeight("Stony Ground", value));
        rockStartHeightSlider.onValueChanged.AddListener(value => SetStartHeight("Rock", value));
        snowStartHeightSlider.onValueChanged.AddListener(value => SetStartHeight("Snow", value));

        textureData.SetBlendStrength(0, waterBlendSlider.value);
        textureData.SetBlendStrength(1, sandyGrassBlendSlider.value);
        textureData.SetBlendStrength(2, grassBlendSlider.value);
        textureData.SetBlendStrength(3, stonyGroundBlendSlider.value);
        textureData.SetBlendStrength(4, rockBlendSlider.value);
        textureData.SetBlendStrength(5, snowBlendSlider.value);
        waterBlendSlider.onValueChanged.AddListener(value => SetBlendStrength("water", value));
        sandyGrassBlendSlider.onValueChanged.AddListener(value => SetBlendStrength("Sandy Grass", value));
        grassBlendSlider.onValueChanged.AddListener(value => SetBlendStrength("Grass", value));
        stonyGroundBlendSlider.onValueChanged.AddListener(value => SetBlendStrength("Stony Ground", value));
        rockBlendSlider.onValueChanged.AddListener(value => SetBlendStrength("Rock", value));
        snowBlendSlider.onValueChanged.AddListener(value => SetBlendStrength("Snow", value));

        textureData.SetTextureScale(0, waterTextureScaleSlider.value);
        textureData.SetTextureScale(1, sandyGrassTextureScaleSlider.value);
        textureData.SetTextureScale(2, grassTextureScaleSlider.value);
        textureData.SetTextureScale(3, stonyGroundTextureScaleSlider.value);
        textureData.SetTextureScale(4, rockTextureScaleSlider.value);
        textureData.SetTextureScale(5, snowTextureScaleSlider.value);
        waterTextureScaleSlider.onValueChanged.AddListener(value => SetTextureScale("water", value));
        sandyGrassTextureScaleSlider.onValueChanged.AddListener(value => SetTextureScale("Sandy Grass", value));
        grassTextureScaleSlider.onValueChanged.AddListener(value => SetTextureScale("Grass", value));
        stonyGroundTextureScaleSlider.onValueChanged.AddListener(value => SetTextureScale("Stony Ground", value));
        rockTextureScaleSlider.onValueChanged.AddListener(value => SetTextureScale("Rock", value));
        snowTextureScaleSlider.onValueChanged.AddListener(value => SetTextureScale("Snow", value));
    }

    void MapPreviewChange(int value)
    {
        mapPreview.SetDrawMode(value);
        mapPreview.DrawMapInEditor();
    }
    void NormalizeModeChange(int value)
    {
        heightMapSettings.SetNormalizeMode(value);
        mapPreview.DrawMapInEditor();
    }
    void FalloffChange(int value)
    {
        heightMapSettings.SetFalloff(value);
        mapPreview.DrawMapInEditor();
    }
    void FlatShadingChange(int value)
    {
        meshSettings.SetFlatShading(value);
        mapPreview.DrawMapInEditor();
    }
    void LODChange(float value)
    {
        mapPreview.editorPreviewLOD = (int)value;
        mapPreview.DrawMapInEditor();
    }
    void NoiseScaleChange(float value)
    {
        heightMapSettings.SetNoiseScale(value);
        mapPreview.DrawMapInEditor();
    }
    void OctavesChange(float value)
    {
        heightMapSettings.SetOctaves((int)value);
        mapPreview.DrawMapInEditor();
    }
    void PersistanceChange(float value)
    {
        heightMapSettings.SetPersistance(value);
        mapPreview.DrawMapInEditor();
    }
    void LacunarityChange(float value)
    {
        heightMapSettings.SetLacunarity(value);
        mapPreview.DrawMapInEditor();
    }
    void SeedChange(float value)
    {
        heightMapSettings.SetSeed((int)value);
        mapPreview.DrawMapInEditor();
    }
    void OffsetXChange(float value)
    {
        heightMapSettings.SetOffsetX(value);
        mapPreview.DrawMapInEditor();
    }
    void OffsetYChange(float value)
    {
        heightMapSettings.SetOffsetY(value);
        mapPreview.DrawMapInEditor();
    }
    void HeightMultiplierChange(float value)
    {
        heightMapSettings.SetHeightMultiplier(value);
        mapPreview.DrawMapInEditor();
    }

    public void SetTintStrength(string area, float value)
    {
        switch (area)
        {
            case "water":
                textureData.SetTintStrength(0, value);
                break;
            case "Sandy Grass":
                textureData.SetTintStrength(1, value);
                break;
            case "Grass":
                textureData.SetTintStrength(2, value);
                break;
            case "Stony Ground":
                textureData.SetTintStrength(3, value);
                break;
            case "Rock":
                textureData.SetTintStrength(4, value);
                break;
            case "Snow":
                textureData.SetTintStrength(5, value);
                break;
        }
        mapPreview.DrawMapInEditor();
    }

    public void SetStartHeight(string area, float value)
    {
        switch (area)
        {
            case "water":
                textureData.SetStartHeight(0, value);
                break;
            case "Sandy Grass":
                textureData.SetStartHeight(1, value);
                break;
            case "Grass":
                textureData.SetStartHeight(2, value);
                break;
            case "Stony Ground":
                textureData.SetStartHeight(3, value);
                break;
            case "Rock":
                textureData.SetStartHeight(4, value);
                break;
            case "Snow":
                textureData.SetStartHeight(5, value);
                break;
        }
        mapPreview.DrawMapInEditor();
    }

    public void SetBlendStrength(string area, float value)
    {
        switch (area)
        {
            case "water":
                textureData.SetBlendStrength(0, value);
                break;
            case "Sandy Grass":
                textureData.SetBlendStrength(1, value);
                break;
            case "Grass":
                textureData.SetBlendStrength(2, value);
                break;
            case "Stony Ground":
                textureData.SetBlendStrength(3, value);
                break;
            case "Rock":
                textureData.SetBlendStrength(4, value);
                break;
            case "Snow":
                textureData.SetBlendStrength(5, value);
                break;
        }
        mapPreview.DrawMapInEditor();
    }

    public void SetTextureScale(string area, float value)
    {
        switch (area)
        {
            case "water":
                textureData.SetTextureScale(0, value);
                break;
            case "Sandy Grass":
                textureData.SetTextureScale(1, value);
                break;
            case "Grass":
                textureData.SetTextureScale(2, value);
                break;
            case "Stony Ground":
                textureData.SetTextureScale(3, value);
                break;
            case "Rock":
                textureData.SetTextureScale(4, value);
                break;
            case "Snow":
                textureData.SetTextureScale(5, value);
                break;
        }
        mapPreview.DrawMapInEditor();
    }
}
