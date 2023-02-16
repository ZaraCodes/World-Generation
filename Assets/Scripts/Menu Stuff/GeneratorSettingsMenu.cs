using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GeneratorSettingsMenu : MonoBehaviour
{
    /// <summary>Main menu reference</summary>
    public MainMenu mainMenu;

    /// <summary>Current custom generator settings</summary>
    [SerializeField] private GameObject customGeneratorSettings;

    /// <summary>Reference to the seed toggle</summary>
    [SerializeField] private Toggle seedToggle;

    /// <summary>Reference to the seed input field</summary>
    [SerializeField] private TMP_InputField seedInputField;

    /// <summary>Reference to the generator world preset</summary>
    [SerializeField] private TMP_Dropdown worldTypeSelector;

    /// <summary>Reference to the respone text</summary>
    [SerializeField] private TMP_Text textResponse;

    /// <summary>The currently selected world generator settings</summary>
    public GeneratorSettings selectedSettings;

    /// <summary>index of the currently selected preset</summary>
    public int selectedPreset;

    /// <summary>Toggles the custom settings menu</summary>
    public void ToggleCustomGeneratorSettings()
    {
        customGeneratorSettings.SetActive(customGeneratorSettings.activeInHierarchy);
    }

    /// <summary>Initializes the settings menu</summary>
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

    /// <summary>Evaluates the choses selection of the generator preset selector</summary>
    /// <param name="selection"></param>
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

    /// <summary>Sets the response text of the settings menu</summary>
    /// <param name="content">Content of the response</param>
    private void SetResponseText(string content)
    {
        textResponse.text = content;
        StopCoroutine(DisableText());
        StartCoroutine(DisableText());
    }

    /// <summary>Disables the display text</summary>
    /// <returns>coroutine idk</returns>
    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(4);
        textResponse.text = string.Empty;
    }

    /// <summary>Resets the seed field</summary>
    /// <param name="reset">bool that resets the field if true</param>
    public void ResetSeedField(bool reset)
    {
        if (!reset) seedInputField.text = string.Empty;
    }

    /// <summary>Resets the settings menu</summary>
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

    /// <summary>Applies the selected settings</summary>
    /// <param name="showText">displays the "settings applied" text if true</param>
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

    /// <summary>shows the main menu and hides the settings menu</summary>
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
