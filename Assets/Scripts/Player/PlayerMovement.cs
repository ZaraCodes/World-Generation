using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Class Variables & Properties
    /// <summary>Reference to the character controller</summary>
    [SerializeField] private CharacterController characterController;

    /// <summary>Reference to the player script</summary>
    [SerializeField] private PlayerScript playerScript;

    /// <summary>Strength of gravity</summary>
    [SerializeField] private float gravityAcceleration = -9.81f;

    /// <summary>Default movement speed</summary>
    [SerializeField] private float walkingSpeed;

    /// <summary>Default running speed</summary>
    [SerializeField] private float runningSpeed;

    /// <summary>Default crouching speed</summary>
    [SerializeField] private float crouchingSpeed;

    /// <summary>Vertical velocity of the player</summary>
    private float velocityY = 0;

    /// <summary>Boolean that controls sprinting speed</summary>
    private bool sprint = false;

    /// <summary>Boolean that controls sprinting speed</summary>
    private bool crouch = false;
    #endregion

    #region Methods
    /// <summary>Moves the player aka Jackson</summary>
    /// <param name="movementVector"></param>
    public void Move(Vector2 movementVector)
    {
        if (characterController.isGrounded) velocityY = 0f;
        else velocityY += gravityAcceleration * Time.deltaTime;

        // speed that will be applied to the player
        float movementSpeed = sprint ? runningSpeed : crouch ? crouchingSpeed : walkingSpeed;

        Vector3 move = new(movementVector.x, velocityY, movementVector.y);
        characterController.Move(movementSpeed * Time.deltaTime * transform.TransformDirection(move));
    }

    /// <summary>Toggles sprinting by an input by the player</summary>
    /// <param name="callbackContext"></param>
    public void ToggleSprint(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame())
        {
            sprint = true;
            if (crouch)
            {
                crouch = false;
                playerScript.camAnimationPlayer.SetBool("Crouch", crouch);
            }
        }
        else if (callbackContext.action.WasReleasedThisFrame())
        {
            sprint = false;
        }
    }

    /// <summary>Toggles crouching by an input by the player</summary>
    /// <param name="callbackContext"></param>
    public void ToggleCrouch(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame())
        {
            crouch = true;
            sprint = false;
            playerScript.camAnimationPlayer.SetBool("Crouch", crouch);
        }
        else if (callbackContext.action.WasReleasedThisFrame())
        {
            crouch = false;
            playerScript.camAnimationPlayer.SetBool("Crouch", crouch);
        }
    }

    public void ToggleLeanLeft(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame())
        {
            playerScript.camAnimationPlayer.SetBool("Lean Left", true);
        }
        else if (callbackContext.action.WasReleasedThisFrame())
        {
            playerScript.camAnimationPlayer.SetBool("Lean Left", false);
        }
    }

    public void ToggleLeanRight(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame())
        {
            playerScript.camAnimationPlayer.SetBool("Lean Right", true);
        }
        else if (callbackContext.action.WasReleasedThisFrame())
        {
            playerScript.camAnimationPlayer.SetBool("Lean Right", false);
        }
    }
    #endregion
}
