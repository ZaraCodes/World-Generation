using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GeneratorSettingsMenu : MonoBehaviour
{
    public MainMenu mainMenu;
    [SerializeField] private GameObject customGeneratorSettings;

    [SerializeField] private Toggle seedToggle;
    [SerializeField] private TMP_InputField seedInputField;
    [SerializeField] private TMP_Dropdown worldTypeSelector;

    [SerializeField] private TMP_Text textResponse;

    public GeneratorSettings selectedSettings;

    public int selectedPreset;

    public void ToggleCustomGeneratorSettings()
    {
        customGeneratorSettings.SetActive(customGeneratorSettings.activeInHierarchy);
    }

    public void Init()
    {
        selectedSettings = mainMenu.currentSettings;
        seedToggle.isOn = GeneratorSettingsSingleton.Instance.useCustomSeed;
        if (seedToggle.isOn)
        {
            seedInputField.text = GeneratorSettingsSingleton.Instance.seed.ToString();
            seedInputField.interactable = true;
        }
        worldTypeSelector.value = GeneratorSettingsSingleton.Instance.SelectedPresetIdx;
    }

    public void EvaluateChosenGeneratorSettings(int selection)
    {
        if (selection < mainMenu.generatorPresets.Length)
        {
            selectedSettings = mainMenu.generatorPresets[selection];
            selectedPreset = selection;
        }
        GeneratorSettingsSingleton.Instance.SelectedPresetIdx = selection;
        if (selection == mainMenu.generatorPresets.Length)
        {
            customGeneratorSettings.SetActive(true);
        }
        else
        {
            customGeneratorSettings.SetActive(false);
        }
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

    public void ResetSeedField(bool reset)
    {
        if (!reset) seedInputField.text = string.Empty;
    }

    public void ResetSettings()
    {
        SetResponseText("Settings reset!");
        customGeneratorSettings.SetActive(false);
        seedInputField.text = string.Empty;
        seedInputField.interactable = false;
        seedToggle.isOn = false;
        GeneratorSettingsSingleton.Instance.useCustomSeed = false;
        GeneratorSettingsSingleton.Instance.SelectedPresetIdx = 0;
        worldTypeSelector.value = 0;
        selectedSettings = mainMenu.generatorPresets[0];
        ApplySettings(false);
    }

    public void ApplySettings(bool showText)
    {
        mainMenu.currentSettings = selectedSettings;
        if (seedToggle.isOn)
        {
            GeneratorSettingsSingleton.Instance.seed = int.TryParse(seedInputField.text, out int seed) ? seed : 0;
            GeneratorSettingsSingleton.Instance.useCustomSeed = true;
        }
        if (showText) SetResponseText("Settings applied!");
    }

    public void BackToMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Init();
    }
}
