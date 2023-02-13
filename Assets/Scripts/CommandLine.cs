using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.IO;

public class CommandLine : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatText;

    [SerializeField] private PlayerScript player;
    // Start is called before the first frame update

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

    public void HideChat()
    {
        chatText.text = string.Empty;

        chatText.DeactivateInputField();
        chatText.gameObject.SetActive(false);
    }

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

    private string TrimTime(int time)
    {
        return time.ToString().PadLeft(2, '0');
    }

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

    private void EvalSeed(string[] args)
    {
        if (args.Length == 1)
        {
            Debug.Log(GeneratorSettingsSingleton.Instance.seed);
        }
    }

    private void EvalTP(string[] args)
    {
        if (args.Length == 3)
        {
            if (float.TryParse(args[1], out float x))
            {
                if (int.TryParse(args[2], out int z))
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
