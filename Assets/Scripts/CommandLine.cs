using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.IO;

public class CommandLine : MonoBehaviour
{
    /// <summary>Reference to the text field of the command line</summary>
    [SerializeField] private TMP_InputField chatText;

    /// <summary>Reference to the player</summary>
    [SerializeField] private PlayerScript player;
    // Start is called before the first frame update

    /// <summary>Enables and makes the command line visible</summary>
    /// <param name="callbackContext">Callback context of the input action</param>
    public void ActivateChat(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame() && !chatText.gameObject.activeInHierarchy)
        {
            player.MovementEnabled = false;
            chatText.gameObject.SetActive(true);
            // chat.SetActive(true);
            chatText.ActivateInputField();
            chatText.Select();
            chatText.text = string.Empty;
        }
    }

    /// <summary>Confirms the command and closes the command line</summary>
    /// <param name="callbackContext">Callback context of the input action</param>
    public void ConfirmChat(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame())
        {
            if (chatText.gameObject.activeInHierarchy)
            {
                chatText.ReleaseSelection();

                print(chatText.text);
                EvaluateCommand(chatText.text);
                HideChat();
                player.MovementEnabled = true;
            }
        }
    }

    /// <summary>Makes the command line invisible</summary>
    public void HideChat()
    {
        chatText.text = string.Empty;

        chatText.DeactivateInputField();
        chatText.gameObject.SetActive(false);
    }

    /// <summary>Takes a screenshot and saves it in the user folder</summary>
    /// <param name="callbackContext">Callback context of the input action</param>
    public void TakeScreenshot(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame())
        {
            if (!Directory.Exists($"{Application.persistentDataPath}\\Screenshots"))
            {
                Directory.CreateDirectory($"{Application.persistentDataPath}\\Screenshots");
            }
            ScreenCapture.CaptureScreenshot($"{Application.persistentDataPath}\\Screenshots\\Screenshot_{TrimTime(System.DateTime.Now.Year)}-{TrimTime(System.DateTime.Now.Month)}-{TrimTime(System.DateTime.Now.Day)}_{TrimTime(System.DateTime.Now.Hour)}-{TrimTime(System.DateTime.Now.Minute)}-{TrimTime(System.DateTime.Now.Second)}.png");
        }
    }

    /// <summary>Fills up a time string to two digits</summary>
    /// <param name="time">The input value</param>
    /// <returns>The modified string</returns>
    private string TrimTime(int time)
    {
        return time.ToString().PadLeft(2, '0');
    }

    /// <summary>Evaluates the user's input</summary>
    /// <param name="command">The user's input</param>
    private void EvaluateCommand(string command)
    {
        string[] splitCommand = command.Split(' ');
        switch (splitCommand[0])
        {
            case "tp": EvalTP(splitCommand); break;
            case "seed": EvalSeed(splitCommand); break;
            default: return;
        }
    }

    /// <summary>
    /// Executes the "seed" command by copying the seed to the clipboard
    /// </summary>
    /// <param name="args">the user's input</param>
    private void EvalSeed(string[] args)
    {
        if (args.Length == 1)
        {
            GUIUtility.systemCopyBuffer = GeneratorSettingsSingleton.Instance.seed.ToString();
        }
    }

    /// <summary>Executes the teleport command</summary>
    /// <param name="args">The user's input</param>
    private void EvalTP(string[] args)
    {
        if (args.Length == 3)
        {
            if (float.TryParse(args[1], out float x))
            {
                if (float.TryParse(args[2], out float z))
                {
                    player.Teleport(x, z);
                }
                else print($"{args[2]} is not a number");
            }
            else print($"{args[1]} is not a number");
        }
        else print("command requires 2 arguments");
    }
}
