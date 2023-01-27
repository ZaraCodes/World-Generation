using UnityEngine;

/// <summary>Class that handles looking around with the mouse</summary>
public class MouseLook : MonoBehaviour
{
    #region Class Variables & Properties
    /// <summary>Horizontal sensitivity</summary>
    [SerializeField] private float sensitivityX = 8f;
    /// <summary>Vertical sensitivity</summary>
    [SerializeField] private float sensitivityY = 8f;
    [Space]
    /// <summary>Reference to the transfrom of the camera</summary>
    [SerializeField] private Transform playerCamera;
    /// <summary>Limits how far the camera can look up and down</summary>
    [SerializeField] private float xClamp = 85f;
    private float xRotation = 0f;

    /// <summary>Horizontal mouse movement</summary>
    private float mouseX;
    /// <summary>Vertical mouse movement</summary>
    private float mouseY;
    #endregion

    #region Methods
    /// <summary>
    /// Moves the camera
    /// </summary>
    /// <param name="mouseInput"></param>
    public void Move(Vector2 mouseInput)
    {
        mouseX = mouseInput.x * sensitivityX;
        mouseY = mouseInput.y * sensitivityY;

        transform.Rotate(Vector3.up, mouseX * Time.deltaTime);

        xRotation -= mouseY * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;

        playerCamera.eulerAngles = targetRotation;
    }
    #endregion
}
