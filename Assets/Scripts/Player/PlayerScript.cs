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

    /// <summary>Reference to the world generator</summary>
    [SerializeField] private Generator generator;
    #endregion

    #region Methods
    /// <summary>
    /// Teleports the player to a desired position
    /// </summary>
    /// <param name="x">new x position in the world</param>
    /// <param name="z">new z position in the world</param>
    public void Teleport(float x, float z)
    {
        playerMovement.characterController.enabled = false;
        Vector3 newPosition = new(x, generator.EvaluateCoordinateHeight(new(x, 0, z)) + 1.1f, z);
        transform.position = newPosition;
        if (transform.position.y < GeneratorSettingsSingleton.Instance.GeneratorSettings.WaterHeight + 1f)
            transform.position = new(transform.position.x, GeneratorSettingsSingleton.Instance.GeneratorSettings.WaterHeight + 1f, transform.position.z);

        playerMovement.characterController.enabled = true;
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
        playerMovement.characterController.enabled = false;
        transform.position = new(Random.Range(-100, 100), 1f, Random.Range(-100, 100));
        transform.position = new(transform.position.x, generator.EvaluateCoordinateHeight(transform.position) + 1f, transform.position.z);
        
        if (transform.position.y < GeneratorSettingsSingleton.Instance.GeneratorSettings.WaterHeight + 1f)
            transform.position = new(transform.position.x, GeneratorSettingsSingleton.Instance.GeneratorSettings.WaterHeight + 1f, transform.position.z);

        playerMovement.characterController.enabled = true;
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
