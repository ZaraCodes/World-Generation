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
    [SerializeField] private Slider octaves;

    /// <summary>Text that displays the current octaves</summary>
    [SerializeField] private TMP_Text octavesDisplay;

    /// <summary>Reference to the lacunarity input field</summary>
    [SerializeField] private Slider lacunarity;
    
    /// <summary>Text that displays the current persistence</summary>
    [SerializeField] private TMP_Text lacunarityDisplay;

    /// <summary>Reference to the persistence input field</summary>
    [SerializeField] private Slider persistence;

    /// <summary>Text that displays the current persistence</summary>
    [SerializeField] private TMP_Text persistenceDisplay;
    
    /// <summary>Reference to the forest threshhold input field</summary>
    [SerializeField] private Slider forestThreshhold;

    /// <summary>Text that displays the current forest threshhold</summary>
    [SerializeField] private TMP_Text forestThreshholdDisplay;

    /// <summary>Reference to the forest frequency input field</summary>
    [SerializeField] private Slider forestFrequency;

    /// <summary>Text that displays the current forest frequency</summary>
    [SerializeField] private TMP_Text forestFrequencyDisplay;

    /// <summary>Reference to the hilliness frequency input field</summary>
    [SerializeField] private Slider hillinessFrequency;

    /// <summary>Text that displays the current hilliness frequency</summary>
    [SerializeField] private TMP_Text hillinessFrequencyDisplay;

    /// <summary>Reference to the base height frequency input field</summary>
    [SerializeField] private TMP_InputField baseHeightFrequency;
    
    /// <summary>Reference to the base height multiplier input field</summary>
    [SerializeField] private TMP_InputField baseHeightMultiplier;
    
    /// <summary>Reference to the base height input field</summary>
    [SerializeField] private TMP_InputField baseHeight;
    
    /// <summary>Reference to the water height input field</summary>
    [SerializeField] private TMP_InputField waterHeight;
    
    /// <summary>Reference to the chunk resolution slider</summary>
    [SerializeField] private Slider chunkResolution;

    /// <summary>Text that displays the current chunk resolution</summary>
    [SerializeField] private TMP_Text chunkResolutionDisplay;
    
    /// <summary>Reference to the chunk size slider</summary>
    [SerializeField] private Slider chunkSize;

    /// <summary>Text that displays the current chunk size</summary>
    [SerializeField] private TMP_Text chunkSizeDisplay;

    /// <summary>Reference to the text response</summary>
    [SerializeField] private TMP_Text textResponse;

    /// <summary>Initializes the custom world settings fields with the currently selected settings values</summary>
    private void InitInputFields()
    {
        noiseStrength.text = settingsMenu.selectedSettings.NoiseStrength.ToString();
        frequency.text = settingsMenu.selectedSettings.Frequency.ToString();
        octaves.value = settingsMenu.selectedSettings.Octaves;
        octavesDisplay.text = settingsMenu.selectedSettings.Octaves.ToString();
        lacunarity.value = settingsMenu.selectedSettings.Lacunarity;
        lacunarityDisplay.text = settingsMenu.selectedSettings.Lacunarity.ToString();
        persistence.value = settingsMenu.selectedSettings.Persistence;
        persistenceDisplay.text = settingsMenu.selectedSettings.Persistence.ToString();
        forestThreshhold.value = settingsMenu.selectedSettings.ForestThreshhold;
        forestFrequency.value = settingsMenu.selectedSettings.ForestFrequency;
        forestFrequencyDisplay.text = (settingsMenu.selectedSettings.ForestFrequency * 100).ToString();
        hillinessFrequency.value = settingsMenu.selectedSettings.HillinessFrequency;
        UpdateHillinessFrequencyDisplay(hillinessFrequency.value);
        baseHeightFrequency.text = settingsMenu.selectedSettings.BaseHeightFrequency.ToString();
        baseHeightMultiplier.text = settingsMenu.selectedSettings.BaseHeightMultiplier.ToString();
        baseHeight.text = settingsMenu.selectedSettings.BaseHeight.ToString();
        waterHeight.text = settingsMenu.selectedSettings.WaterHeight.ToString();
        chunkResolution.value = settingsMenu.selectedSettings.ChunkResolution;
        chunkResolutionDisplay.text = settingsMenu.selectedSettings.ChunkResolution.ToString();
        chunkSize.value = settingsMenu.selectedSettings.ChunkSize;
        chunkSizeDisplay.text = settingsMenu.selectedSettings.ChunkSize.ToString();
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
            settings.Octaves = (int) octaves.value;
            settings.Lacunarity = lacunarity.value;
            settings.Persistence = persistence.value;
            settings.ForestThreshhold = forestThreshhold.value;
            settings.ForestFrequency = forestFrequency.value;
            settings.HillinessFrequency = hillinessFrequency.value;
            settings.BaseHeightFrequency = float.Parse(baseHeightFrequency.text);
            settings.BaseHeightMultiplier = float.Parse(baseHeightMultiplier.text);
            settings.BaseHeight = float.Parse(baseHeight.text);
            settings.WaterHeight = float.Parse(waterHeight.text);
            settings.ChunkResolution = (int) chunkResolution.value;
            settings.ChunkSize = (int) chunkSize.value;

            settingsMenu.selectedSettings = settings;

            SetResponseText("Settings applied!");
            settingsMenu.ApplySettings(true);
        }
        catch
        {
            SetResponseText("Applying Settings failed!");
        }
    }

    public void UpdateChunkResolutionDisplay(float value)
    {
        chunkResolutionDisplay.text = ((int) value).ToString();
    }

    public void UpdateChunkSizeDisplay(float value)
    {
        chunkSizeDisplay.text = ((int) value).ToString();
    }

    public void UpdateForestThreshholdDisplay(float value)
    {
        forestThreshholdDisplay.text = value > 0 ? value.ToString().Length > 4 ? value.ToString().Substring(0, 4) : value.ToString() : value.ToString().Length > 5 ? value.ToString().Substring(0, 5) : value.ToString();
    }

    public void UpdatePersistenceDisplay(float value)
    {
        persistenceDisplay.text = value.ToString().Length > 4 ? value.ToString().Substring(0, 4) : value.ToString();
    }

    public void UpdateLacunarityDisplay(float value)
    {
        lacunarityDisplay.text = value.ToString().Length > 4 ? value.ToString().Substring(0, 4) : value.ToString();
    }

    public void UpdateOctavesDisplay(float value)
    {
        octavesDisplay.text = ((int) value).ToString();
    }

    public void UpdateForestFrequencyDisplay(float value)
    {
        value *= 100;
        forestFrequencyDisplay.text = value.ToString().Length > 4 ? value.ToString().Substring(0, 5) : value.ToString();
    }

    public void UpdateHillinessFrequencyDisplay(float value)
    {
        hillinessFrequencyDisplay.text = value.ToString().Length > 4 ? value.ToString().Substring(0, 4) : value.ToString();
    }
}
