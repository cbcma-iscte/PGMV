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


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ProcessLook(float mouseX, float mouseY)
    {

        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = math.clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = quaternion.Euler(xRotation,0,0);

        transform.Rotate(Vector3.up * (mouseX *Time.deltaTime) * xSensitivity);
    }

}
