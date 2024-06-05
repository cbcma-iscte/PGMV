using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] public Camera cam;
    private float xRotation = 0;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    public float maxVerticalAngle = 80f;
    public float minVerticalAngle = -80f;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnableCursor()
    {
         Cursor.lockState = CursorLockMode.None;
    }

    public void ProcessLook(float mouseX, float mouseY)
    {

        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * mouseX *Time.deltaTime * xSensitivity);
    }

}
