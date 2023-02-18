using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    /// <summary>Reference to the pause menu gameobject</summary>
    [SerializeField] private GameObject pauseMenu;

    /// <summary>Reference to the command line</summary>
    [SerializeField] private TMP_InputField commandLine;

    /// <summary>Reference to the player</summary>
    [SerializeField] private PlayerScript player;

    /// <summary>Opens/Closes the pause menu</summary>
    /// <param name="callbackContext">Callback context of the input action</param>
    public void TogglePauseMenu(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame())
        {
            commandLine.text = string.Empty;
            commandLine.gameObject.SetActive(false);

            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
            if (pauseMenu.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.None;
                player.MovementEnabled = false;
                commandLine.gameObject.GetComponent<CommandLine>().HideChat();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                player.MovementEnabled = true;
            }
        }
    }

    /// <summary>Closes the pause menu</summary>
    public void ClosePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            player.MovementEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /// <summary>Quits back to the main menu</summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>Reloads the world generator scene with the current generator settings</summary>
    public void RegenerateWorld()
    {
        SceneManager.LoadScene(1);
    }
}
