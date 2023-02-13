using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorSettingsMenu : MonoBehaviour
{
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private GameObject customGeneratorSettings;

    [SerializeField] private Toggle seedToggle;
    [SerializeField] private TMP_InputField seedInputField;
    [SerializeField] private TMP_Dropdown GeneratorSettingsDropdown;

    public GeneratorSettings selectedSettings;

    public void ToggleCustomGeneratorSettings()
    {
        customGeneratorSettings.SetActive(customGeneratorSettings.activeInHierarchy);
    }

    public void EvaluateChosenGeneratorSettings(int selection)
    {
        if (selection == 2)
        {
            customGeneratorSettings.SetActive(true);
        }
        else
        {
            customGeneratorSettings.SetActive(false);
        }
    }

    public void ResetSeedField(bool reset)
    {
        if (!reset) seedInputField.text = string.Empty;
    }

    public void ResetSettings()
    {
        customGeneratorSettings.SetActive(false);
        seedInputField.text = string.Empty;
        seedInputField.interactable = false;
        seedToggle.isOn = false;
    }

    public void ApplySettings()
    {
        mainMenu.currentSettings = selectedSettings;
        if (seedToggle.isOn)
        {
            GeneratorSettingsSingleton.Instance.seed = int.TryParse(seedInputField.text, out int seed) ? seed : 0;
            GeneratorSettingsSingleton.Instance.useCustomSeed = true;
        }
    }

    public void BackToMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
