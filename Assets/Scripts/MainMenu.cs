using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GeneratorSettings currentSettings;
    public GeneratorSettings[] generatorPresets;

    [SerializeField] private GameObject settingsMenu;

    public void ExecuteStartButton()
    {
        GeneratorSettingsSingleton.Instance.GeneratorSettings = currentSettings;
        SceneManager.LoadScene(1);
    }

    public void ShowSettigns()
    {
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<GeneratorSettingsMenu>().selectedSettings = currentSettings;
        gameObject.SetActive(false);
    }

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
