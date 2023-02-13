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

    [SerializeField] private TMP_Text textResponse;

    public GeneratorSettings selectedSettings;

    public void ToggleCustomGeneratorSettings()
    {
        customGeneratorSettings.SetActive(customGeneratorSettings.activeInHierarchy);
    }

    public void Init()
    {
        seedToggle.isOn = GeneratorSettingsSingleton.Instance.useCustomSeed;
        if (seedToggle.isOn)
        {
            seedInputField.text = GeneratorSettingsSingleton.Instance.seed.ToString();
        }
    }

    public void EvaluateChosenGeneratorSettings(int selection)
    {
        if (selection < mainMenu.generatorPresets.Length)
        {
            selectedSettings = mainMenu.generatorPresets[selection];
        }
        if (selection == 2)
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
    }

    public void ApplySettings()
    {
        mainMenu.currentSettings = selectedSettings;
        if (seedToggle.isOn)
        {
            GeneratorSettingsSingleton.Instance.seed = int.TryParse(seedInputField.text, out int seed) ? seed : 0;
            GeneratorSettingsSingleton.Instance.useCustomSeed = true;
        }
        SetResponseText("Settings applied!");
    }

    public void BackToMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
