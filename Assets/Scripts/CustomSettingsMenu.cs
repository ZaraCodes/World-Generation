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
    [SerializeField] private TMP_InputField chunkResolution;
    [SerializeField] private TMP_InputField chunkSize;

    [SerializeField] private TMP_Text textResponse;

    private void InitInputFields()
    {
        noiseStrength.text = defaultSettings.NoiseStrength.ToString();
        frequency.text = defaultSettings.Frequency.ToString();
        octaves.text = defaultSettings.Octaves.ToString();
        lacunarity.text = defaultSettings.Lacunarity.ToString();
        persistence.text = defaultSettings.Persistence.ToString();
        hillinessFrequency.text = defaultSettings.HillinessFrequency.ToString();
        baseHeightFrequency.text = defaultSettings.BaseHeightFrequency.ToString();
        baseHeightMultiplier.text = defaultSettings.BaseHeightMultiplier.ToString();
        baseHeight.text = defaultSettings.BaseHeight.ToString();
        chunkResolution.text = defaultSettings.ChunkResolution.ToString();
        chunkSize.text = defaultSettings.ChunkSize.ToString();
    }

    public void ResetSettings()
    {
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
