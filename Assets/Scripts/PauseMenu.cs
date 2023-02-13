using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TMP_InputField commandLine;
    [SerializeField] private PlayerScript player;

    public void TogglePauseMenu(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame())
        {
            commandLine.text = string.Empty;
            commandLine.gameObject.SetActive(false);

            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
            if (pauseMenu.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Confined;
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

    public void ClosePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            player.MovementEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RegenerateWorld()
    {
        SceneManager.LoadScene(1);
    }
}
