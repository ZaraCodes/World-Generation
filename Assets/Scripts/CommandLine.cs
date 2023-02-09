using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.EventSystems;

public class CommandLine : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatText;

    [SerializeField] private GameObject chat;

    [SerializeField] private PlayerScript player;
    // Start is called before the first frame update

    public void ActivateChat(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame() && !chat.activeInHierarchy)
        {
            chat.SetActive(true);
            chatText.ActivateInputField();
            chatText.Select();
        }
    }

    public void ConfirmChat(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame())
        {
            if (chat.activeInHierarchy)
            {
                chatText.ReleaseSelection();

                print(chatText.text);
                EvaluateCommand(chatText.text);
                chatText.text = string.Empty;

                chatText.DeactivateInputField();
                chat.SetActive(false);
            }
        }
    }

    private void EvaluateCommand(string command)
    {
        string[] splitCommand = command.Split(' ');
        switch (splitCommand[0])
        {
            case "tp": EvalTP(splitCommand); break;
            default: return;
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
