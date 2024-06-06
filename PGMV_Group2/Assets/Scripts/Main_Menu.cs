using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The class Main_Menu is responsible for managing the MainMenu Scene states and button interactions 
/// </summary>
public class Main_Menu : MonoBehaviour
{
    [SerializeField]
    public GameObject InstructionsScreen;
    [SerializeField]
    public GameObject all_Buttons;
    private bool isInstructionsShowing;

    /// <summary>
    /// Initializes the main menu state by hiding the instructions screen and displaying all buttons.
    /// </summary>
    void Start(){
        isInstructionsShowing=false;
        InstructionsScreen.SetActive(isInstructionsShowing);
        all_Buttons.SetActive(!isInstructionsShowing);
    }

    /// <summary>
    /// Loads the "LivingRoom" scene, starting the game.
    /// </summary>
    public void playGame(){
          SceneManager.LoadScene("LivingRoom");
    }

    /// <summary>
    /// Exits the application when the quit button is pressed.
    /// </summary>
    public void quitGame(){
        Application.Quit();

    }

    /// <summary>
    /// Toggles the visibility of the instructions screen and all buttons, allowing users to view or hide the instructions.
    /// </summary>
    public void showInstructions(){
        isInstructionsShowing=!isInstructionsShowing;
        InstructionsScreen.SetActive(isInstructionsShowing);
        all_Buttons.SetActive(!isInstructionsShowing);
    }


}
