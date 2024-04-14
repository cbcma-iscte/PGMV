using UnityEngine;

public class LampToggle : MonoBehaviour
{
    public GameObject lampObject;
    private Light lampLight;
    private bool isLampOn = true;

    void Start()
    {
        lampLight = lampObject.GetComponent<Light>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Change this to whatever input you want
        {
            ToggleLamp();
        }
    }

    void ToggleLamp()
    {
        isLampOn = !isLampOn;
        lampLight.enabled = isLampOn;
    }
}
