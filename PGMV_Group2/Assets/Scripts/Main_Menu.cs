using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    [SerializeField]
    public GameObject InstructionsScreen;
    [SerializeField]
    public GameObject all_Buttons;
    private bool isInstructionsShowing;

    void Start(){
        isInstructionsShowing=false;
        InstructionsScreen.SetActive(isInstructionsShowing);
        all_Buttons.SetActive(!isInstructionsShowing);
    }
    public void playGame(){
          SceneManager.LoadScene("LivingRoom");
    }

    public void quitGame(){
        Application.Quit();

    }

    public void showInstructions(){
        isInstructionsShowing=!isInstructionsShowing;
        InstructionsScreen.SetActive(isInstructionsShowing);
        all_Buttons.SetActive(!isInstructionsShowing);
    }


}
