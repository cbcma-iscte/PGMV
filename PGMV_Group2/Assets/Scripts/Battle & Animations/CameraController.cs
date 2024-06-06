using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Controls the camera movement based on mouse input for a first-person or third-person perspective.
/// </summary>
public class CameraController : MonoBehaviour
{
    
    [SerializeField] public Camera cam;
    private float xRotation = 0;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    public float maxVerticalAngle = 80f;
    public float minVerticalAngle = -80f;

    /// <summary>
    /// Initializes the camera controller by locking the cursor.
    /// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Enables the cursor by unlocking it.
    /// </summary>
    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Processes the mouse input to rotate the camera.
    /// </summary>
    /// <param name="mouseX">The horizontal mouse input.</param>
    /// <param name="mouseY">The vertical mouse input.</param>
    public void ProcessLook(float mouseX, float mouseY)
    {
         // Calculate the new rotation around the X-axis
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        // Clamp the rotation to prevent over-rotation
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        // Apply the rotation to the camera's local rotation
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Rotate the player (or camera parent) around the Y-axis
        transform.Rotate(Vector3.up * mouseX *Time.deltaTime * xSensitivity);
    }

}
