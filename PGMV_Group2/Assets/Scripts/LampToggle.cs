using UnityEngine;

/// <summary>
/// The LampToggle class handles the toggling of a lamp's light component on and off.
/// </summary>
public class LampToggle : MonoBehaviour
{
    public GameObject lampObject;
    private Light lampLight;
    private bool isLampOn = true;

    /// <summary>
    /// Start is called before the first frame update.
    /// Gets the Light component from the lampObject.
    /// </summary>
    void Start()
    {
        lampLight = lampObject.GetComponent<Light>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// Check for space key press to toggle the lamp.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleLamp();
        }
    }

    /// <summary>
    /// Toggles the lamp's light on and off.
    /// </summary>
    void ToggleLamp()
    {
        isLampOn = !isLampOn;
        lampLight.enabled = isLampOn;
    }
}
