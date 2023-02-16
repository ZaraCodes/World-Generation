using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomSettingsMenu : MonoBehaviour
{
    /// <summary>Reference to the settings menu</summary>
    [SerializeField] private GeneratorSettingsMenu settingsMenu;

    /// <summary>Reference to the default settings</summary>
    [SerializeField] private GeneratorSettings defaultSettings;

    /// <summary>Reference to the noise strength input field</summary>
    [SerializeField] private TMP_InputField noiseStrength;

    /// <summary>Reference to the frequency input field</summary>
    [SerializeField] private TMP_InputField frequency;
    
    /// <summary>Reference to the octaves input field</summary>
    [SerializeField] private TMP_InputField octaves;
    
    /// <summary>Reference to the lacunarity input field</summary>
    [SerializeField] private TMP_InputField lacunarity;
    
    /// <summary>Reference to the persistence input field</summary>
    [SerializeField] private TMP_InputField persistence;
    
    /// <summary>Reference to the forest threshhold input field</summary>
    [SerializeField] private TMP_InputField forestThreshhold;
    
    /// <summary>Reference to the forest frequency input field</summary>
    [SerializeField] private TMP_InputField forestFrequency;
    
    /// <summary>Reference to the hilliness frequency input field</summary>
    [SerializeField] private TMP_InputField hillinessFrequency;
    
    /// <summary>Reference to the base height frequency input field</summary>
    [SerializeField] private TMP_InputField baseHeightFrequency;
    
    /// <summary>Reference to the base height multiplier input field</summary>
    [SerializeField] private TMP_InputField baseHeightMultiplier;
    
    /// <summary>Reference to the base height input field</summary>
    [SerializeField] private TMP_InputField baseHeight;
    
    /// <summary>Reference to the water height input field</summary>
    [SerializeField] private TMP_InputField waterHeight;
    
    /// <summary>Reference to the chunk resolution input field</summary>
    [SerializeField] private TMP_InputField chunkResolution;
    
    /// <summary>Reference to the chunk size input field</summary>
    [SerializeField] private TMP_InputField chunkSize;

    /// <summary>Reference to the input field</summary>
    [SerializeField] private TMP_Text textResponse;

    /// <summary>Initializes the custom world settings fields with the currently selected settings values</summary>
    private void InitInputFields()
    {
        noiseStrength.text = settingsMenu.selectedSettings.NoiseStrength.ToString();
        frequency.text = settingsMenu.selectedSettings.Frequency.ToString();
        octaves.text = settingsMenu.selectedSettings.Octaves.ToString();
        lacunarity.text = settingsMenu.selectedSettings.Lacunarity.ToString();
        persistence.text = settingsMenu.selectedSettings.Persistence.ToString();
        forestThreshhold.text = settingsMenu.selectedSettings.ForestThreshhold.ToString();
        forestFrequency.text = settingsMenu.selectedSettings.ForestFrequency.ToString();
        hillinessFrequency.text = settingsMenu.selectedSettings.HillinessFrequency.ToString();
        baseHeightFrequency.text = settingsMenu.selectedSettings.BaseHeightFrequency.ToString();
        baseHeightMultiplier.text = settingsMenu.selectedSettings.BaseHeightMultiplier.ToString();
        baseHeight.text = settingsMenu.selectedSettings.BaseHeight.ToString();
        waterHeight.text = settingsMenu.selectedSettings.WaterHeight.ToString();
        chunkResolution.text = settingsMenu.selectedSettings.ChunkResolution.ToString();
        chunkSize.text = settingsMenu.selectedSettings.ChunkSize.ToString();
    }

    /// <summary>Resets the custom settings</summary>
    public void ResetSettings()
    {
        settingsMenu.selectedSettings = settingsMenu.mainMenu.generatorPresets[settingsMenu.selectedPreset];
        InitInputFields();
        SetResponseText("Settings reset!");
    }

    /// <summary>Sets the response text</summary>
    /// <param name="content">The string that should be displayed</param>
    private void SetResponseText(string content)
    {
        textResponse.text = content;
        StopCoroutine(DisableText());
        StartCoroutine(DisableText());
    }

    /// <summary>Disables the response text</summary>
    /// <returns></returns>
    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(4);
        textResponse.text = string.Empty;
    }

    private void OnEnable()
    {
        InitInputFields();
    }

    /// <summary>Applies the custom settigns</summary>
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
            settings.ForestThreshhold = float.Parse(forestThreshhold.text);
            settings.ForestFrequency = float.Parse(forestFrequency.text);
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
