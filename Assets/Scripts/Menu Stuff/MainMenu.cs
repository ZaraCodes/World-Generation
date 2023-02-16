using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /// <summary>The currently selected settings</summary>
    public GeneratorSettings currentSettings;

    /// <summary>This array contains all world generator presets</summary>
    public GeneratorSettings[] generatorPresets;

    /// <summary>Reference to the settings menu</summary>
    [SerializeField] private GameObject settingsMenu;

    /// <summary>Starts the world generator</summary>
    public void ExecuteStartButton()
    {
        GeneratorSettingsSingleton.Instance.GeneratorSettings = currentSettings;
        SceneManager.LoadScene(1);
    }

    /// <summary>Opens the settings menu</summary>
    public void ShowSettigns()
    {
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<GeneratorSettingsMenu>().selectedSettings = currentSettings;
        gameObject.SetActive(false);
    }

    /// <summary>Closes the game</summary>
    public void ExecuteQuitButton()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void Awake()
    {
        if (GeneratorSettingsSingleton.Instance.GeneratorSettings != null)
        {
            currentSettings = GeneratorSettingsSingleton.Instance.GeneratorSettings;
        }
    }
}
