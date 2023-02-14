using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomSettingsMenu : MonoBehaviour
{
    [SerializeField] private GeneratorSettingsMenu settingsMenu;
    [SerializeField] private GeneratorSettings defaultSettings;

    [SerializeField] private TMP_InputField noiseStrength;
    [SerializeField] private TMP_InputField frequency;
    [SerializeField] private TMP_InputField octaves;
    [SerializeField] private TMP_InputField lacunarity;
    [SerializeField] private TMP_InputField persistence;
    [SerializeField] private TMP_InputField hillinessFrequency;
    [SerializeField] private TMP_InputField baseHeightFrequency;
    [SerializeField] private TMP_InputField baseHeightMultiplier;
    [SerializeField] private TMP_InputField baseHeight;
    [SerializeField] private TMP_InputField waterHeight;
    [SerializeField] private TMP_InputField chunkResolution;
    [SerializeField] private TMP_InputField chunkSize;

    [SerializeField] private TMP_Text textResponse;

    private void InitInputFields()
    {
        noiseStrength.text = settingsMenu.selectedSettings.NoiseStrength.ToString();
        frequency.text = settingsMenu.selectedSettings.Frequency.ToString();
        octaves.text = settingsMenu.selectedSettings.Octaves.ToString();
        lacunarity.text = settingsMenu.selectedSettings.Lacunarity.ToString();
        persistence.text = settingsMenu.selectedSettings.Persistence.ToString();
        hillinessFrequency.text = settingsMenu.selectedSettings.HillinessFrequency.ToString();
        baseHeightFrequency.text = settingsMenu.selectedSettings.BaseHeightFrequency.ToString();
        baseHeightMultiplier.text = settingsMenu.selectedSettings.BaseHeightMultiplier.ToString();
        baseHeight.text = settingsMenu.selectedSettings.BaseHeight.ToString();
        waterHeight.text = settingsMenu.selectedSettings.WaterHeight.ToString();
        chunkResolution.text = settingsMenu.selectedSettings.ChunkResolution.ToString();
        chunkSize.text = settingsMenu.selectedSettings.ChunkSize.ToString();
    }

    public void ResetSettings()
    {
        settingsMenu.selectedSettings = settingsMenu.mainMenu.generatorPresets[settingsMenu.selectedPreset];
        InitInputFields();
        SetResponseText("Settings reset!");
    }

    private void SetResponseText(string content)
    {
        textResponse.text = content;
        StopCoroutine(DisableText());
        StartCoroutine(DisableText());
    }

    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(4);
        textResponse.text = string.Empty;
    }

    private void OnEnable()
    {
        InitInputFields();
    }

    public void ApplySettings()
    {
        try
        {
            GeneratorSettings settings = ScriptableObject.CreateInstance<GeneratorSettings>();
            settings.NoiseStrength = float.Parse(noiseStrength.text);
            settings.Frequency = float.Parse(frequency.text);
            settings.Octaves = int.Parse(octaves.text);
            settings.Lacunarity = float.Parse(lacunarity.text);
            settings.Persistence = float.Parse(persistence.text);
            settings.HillinessFrequency = float.Parse(hillinessFrequency.text);
            settings.BaseHeightFrequency = float.Parse(baseHeightFrequency.text);
            settings.BaseHeightMultiplier = float.Parse(baseHeightMultiplier.text);
            settings.BaseHeight = float.Parse(baseHeight.text);
            settings.WaterHeight = float.Parse(waterHeight.text);
            settings.ChunkResolution = int.Parse(chunkResolution.text);
            settings.ChunkSize = int.Parse(chunkSize.text);

            settingsMenu.selectedSettings = settings;

            SetResponseText("Settings applied!");
        }
        catch
        {
            SetResponseText("Applying Settings failed!");
        }
    }
}
