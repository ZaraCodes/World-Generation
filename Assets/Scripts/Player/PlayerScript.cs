using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    #region Class Variables & Properties
    /// <summary>Reference to the MouseLook script</summary>
    [SerializeField] private MouseLook mouseLook;

    /// <summary>Reference to Jackson's Movement Script</summary>
    [SerializeField] private PlayerMovement playerMovement;

    /// <summary>Reference to the animation player</summary>
    public Animator camAnimationPlayer;

    /// <summary>Enables the movement of the mouse</summary>
    public bool MovementEnabled = true;

    /// <summary>Reference to the controls of the game</summary>
    private WorldGenControls controls;

    /// <summary>Mouse input vector</summary>
    private Vector2 mouseInput;

    /// <summary>Movement vector</summary>
    private Vector2 movementVector;

    #endregion

    #region Methods
    /// <summary>
    /// Quits the game
    /// </summary>
    /// <param name="callbackContext">Callback context if the input</param>
    public void QuitGame(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.WasPerformedThisFrame()) {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }

    /// <summary>
    /// Enables or disables player movement inputs
    /// </summary>
    /// <param name="active">boolean that sets enables or disables movement</param>
    public void SetMovementActive(bool active)
    {
        MovementEnabled = active;
    }
    #endregion

    #region Unity Loop
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MovementEnabled)
        {
            mouseLook.Move(mouseInput);             // look around
            playerMovement.Move(movementVector);    // move around
        }
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // Disables the mouse

        controls = new();   // Creates a new instance of the controls
        controls.JacksonControls.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        controls.JacksonControls.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
        controls.JacksonControls.Movement.performed += ctx => movementVector = ctx.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        controls.Enable();  // Enables the controls 
    }

    private void OnDestroy()
    {
        controls.Disable(); // Disables the class when the player object gets destroyed
    }
    #endregion
}
