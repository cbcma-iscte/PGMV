using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
//using UnityEngine.UIElements;
using UnityEngine.UI;

///<summary> 
///The Menu class is a controller for managing game states and the user interactions in a game
///</summary>
public class Menu : MonoBehaviour
{
    public bool isPlayingAutomatic = false;
    
    public bool isLoadingScenes = false;
    private bool isRestarting = false;

    private bool isMenuOpen = false;
    private bool isPaused = false;
    public GameObject[] Games;
    public string[] players;
    private int i;
    [SerializeField]
    public GameObject allButtons;
    [SerializeField]
    public Toggle automaticCheck;

    [SerializeField]
    public GameObject play_button;
    [SerializeField]
    public GameObject forward_button;

    [SerializeField]
    public GameObject back_button;
    
    [SerializeField]
    public GameObject pause_button;
    

    [SerializeField] public GameObject MenuScreen;
    [SerializeField] public GameObject PauseScreen;

    [SerializeField] public TextMeshProUGUI Turns;
    
    public void Awake(){
        DontDestroyOnLoad(gameObject);
    }
    public void Start(){
        i = 0;
        allButtons.SetActive(true);
        back_button.SetActive(!isPlayingAutomatic);
        forward_button.SetActive(!isPlayingAutomatic);
        
        if(isPaused) isPaused = false;
        if(isMenuOpen) isMenuOpen = false;
        PauseScreen.SetActive(false);
        MenuScreen.SetActive(false);
        Games = GameObject.FindGameObjectsWithTag("GameController");
    }

    /// <summary>
    /// Starts or pauses the game and updates the pause/play button states.
    /// </summary>
    public void start_pause_Game(){
        if(isMenuOpen) isMenuOpen = false;
        isPaused = !isPaused;
        PauseScreen.SetActive(isPaused);
        pause_button.SetActive(!isPaused);
        play_button.SetActive(isPaused);
        foreach(GameObject game in Games)
        {   

            if(isPlayingAutomatic){
                game.GetComponent<GameManager>().PauseResumeGame();
        
            }else{
                game.GetComponent<GameManager>().PauseResumeGame(); 
            }
           
        }
        
    }

    /// <summary>
    /// Restarts the game and resets all relevant variables and UI elements.
    /// </summary>
    public void restart(){
      
        i = 0;
        Turns.text = "Turns: " + i; 
        isPlayingAutomatic =false;

        back_button.SetActive(!isPlayingAutomatic);
        forward_button.SetActive(!isPlayingAutomatic);
        
        foreach(GameObject game in Games)
        {
            game.GetComponent<GameManager>().isAutomatic = isPlayingAutomatic;
            game.GetComponent<GameManager>().RestartGame();
        }
        if(isMenuOpen) isMenuOpen = false;
        MenuScreen.SetActive(isMenuOpen);
        if(isPaused) isPaused = false;
        PauseScreen.SetActive(isPaused);
        allButtons.SetActive(true);
        isRestarting = false;
        
        
    }

    /// <summary>
    /// Goes back one play in the game and updates the turn display.
    /// </summary>
    public void back1play(){

        //see the previous play
        if (i > 0){
            i--;
            Turns.text = "Turns: " + i;
            foreach(GameObject game in Games)
            {
                game.GetComponent<GameManager>().GoBack();
            }
        }

    }

    /// <summary>
    /// Goes forward one play in the game and updates the turn display.
    /// </summary>
    public void forward1play(){
        
        if(verifyTurns()){
        int playing = 0;
        
        foreach(GameObject game in Games){
            if(game.GetComponent<GameManager>().isPlaying==true){
                playing = playing +1;
            }
        }
        if(playing == 0){i++;
        Turns.text = "Turns: " + i;
        foreach(GameObject game in Games)
        {
            if(i>game.GetComponent<GameManager>().Turns.Count){
                isAutomaticToggle();    
            }
            game.GetComponent<GameManager>().GoForward();
            
        }
        }
        }else{
            restart();
            SceneManager.LoadScene("MainMenu");
        
        }
        
        
    }

    /// <summary>
    /// Verifies if there are more turns available to play.
    /// </summary>
    /// <returns>True if there are more turns available, false otherwise.</returns>
    private bool verifyTurns(){
        bool canContinue = false;
        foreach(GameObject game in Games)
        {
            if(i+1<game.GetComponent<GameManager>().Turns.Count){
                canContinue = true;
            } 
        }
        return canContinue;
    }

    /// <summary>
    /// Shows or hides the menu and pauses/resumes the game.
    /// </summary>
    public void showMenu(){
        automaticCheck.isOn = isPlayingAutomatic;
        isMenuOpen = !isMenuOpen;
        allButtons.SetActive(!isMenuOpen);
        MenuScreen.SetActive(isMenuOpen);
        
        isPaused = !isPaused;
        foreach(GameObject game in Games)
        {   
            game.GetComponent<GameManager>().PauseResumeGame(); 
        }
     

    }

    /// <summary>
    /// Toggles the scene loading setting and updates the GameManager instances.
    /// </summary>
    public void isSceneToggle(){
        isLoadingScenes = !isLoadingScenes;
        Debug.Log("Load scenes" + isLoadingScenes);
        foreach(GameObject game in Games)
        {
            game.GetComponent<GameManager>().isLoadingScenes = isLoadingScenes;
        }
    }

    /// <summary>
    /// Toggles the automatic play setting and updates the GameManager instances.
    /// </summary>
    public void isAutomaticToggle(){
        isPlayingAutomatic = !isPlayingAutomatic;
        foreach(GameObject game in Games)
        {
            game.GetComponent<GameManager>().isAutomatic = isPlayingAutomatic;
            
        }
        back_button.SetActive(!isPlayingAutomatic);
        forward_button.SetActive(!isPlayingAutomatic);
    }
    
    /// <summary>
    /// Quits the game and loads the main menu scene.
    /// </summary>
    public void QuitGame(){
          SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Updates the game state based on user input and automatic play setting.
    /// </summary>
    void Update(){

        if(Input.GetKeyDown(KeyCode.P)){start_pause_Game();}
        if(Input.GetKeyDown(KeyCode.RightArrow) && isPlayingAutomatic == false){forward1play();}
        if(Input.GetKeyDown(KeyCode.LeftArrow) && isPlayingAutomatic == false){back1play();}
        if(Input.GetKeyDown(KeyCode.R)){isRestarting = true; restart();}
        if(Input.GetKeyDown(KeyCode.Escape)){showMenu();}
        if(isPlayingAutomatic && isPaused == false){ 
            forward1play();
        }
    }
    
}
