using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    public bool isPlayingAutomatic = false;

    public bool isLoadingScenes = false;

    private bool isMenuOpen = false;
    private bool isPaused = false;
    public GameObject[] Games;
    public string[] players;
    private int i;
    
    [SerializeField]
    public GameObject play_button;
    
    [SerializeField]
    public GameObject pause_button;
    

    [SerializeField] public GameObject MenuScreen;
    [SerializeField] public GameObject PauseScreen;

    [SerializeField] public TextMeshProUGUI Turns;
    
    public void Start(){
        i = 0;
        if(isPaused) isPaused = false;
        if(isMenuOpen) isMenuOpen = false;
        PauseScreen.SetActive(false);
        MenuScreen.SetActive(false);
        Games = GameObject.FindGameObjectsWithTag("GameController");
    }
    public void start_pause_Game(){
        
        if(isMenuOpen) isMenuOpen = false;
        isPaused = !isPaused;
        PauseScreen.SetActive(isPaused);
        foreach(GameObject game in Games)
        {   

            if(isPlayingAutomatic){
                game.GetComponent<GameManager>().PauseResumeGame();
        
            }else{
                game.GetComponent<GameManager>().PauseResumeGame(); 
            }
           
        }
        
    }

    public void restart(){
        if(isMenuOpen) isMenuOpen = false;
        MenuScreen.SetActive(isMenuOpen);
        if(isPaused) isPaused = false;
        PauseScreen.SetActive(isPaused);
        i=0;
        Turns.text = "Turns: " + i;
        foreach(GameObject game in Games)
        {
            game.GetComponent<GameManager>().RestartGame();
        }
    }

    public void back1play(){

        //see the previous play
        i--;
        Turns.text = "Turns: " + i;
        foreach(GameObject game in Games)
        {
            game.GetComponent<GameManager>().GoBack();
        }

    }

    public void forward1play(){
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
            game.GetComponent<GameManager>().GoForward();
            
        }
        }
        
        
    }

    public void killedEnemy(string player){
        if(player.Equals("player1")){
            //player1Points.text = " " +(currentValuePlayer1 + 1).ToString() +" points";
            //currentValuePlayer1++;
        }
        else{
           // player2Points.text = " "+(currentValuePlayer2 + 1).ToString() +" points";
           // currentValuePlayer2++;
        }
    }

    public void showMenu(){
       
        isMenuOpen = !isMenuOpen;
        MenuScreen.SetActive(isMenuOpen);
        
        isPaused = !isPaused;
        foreach(GameObject game in Games)
        {   

            if(isPlayingAutomatic){
                game.GetComponent<GameManager>().PauseResumeGame();
        
            }else{
                game.GetComponent<GameManager>().PauseResumeGame(); 
            }
            
        }
    }


    public void isAutomaticToggle(){
        isPlayingAutomatic = !isPlayingAutomatic;
    }

    public void isLoadingBattles(){
        isLoadingScenes = !isLoadingScenes;
    }

    public void QuitGame(){
        Application.Quit();
    }

    void Update(){
        
        if(Input.GetKeyDown(KeyCode.P)){start_pause_Game();}
        if(Input.GetKeyDown(KeyCode.RightArrow) && !isPlayingAutomatic){forward1play();}
        if(Input.GetKeyDown(KeyCode.LeftArrow) && !isPlayingAutomatic){back1play();}
        if(Input.GetKeyDown(KeyCode.R)){restart();}
        if(Input.GetKeyDown(KeyCode.Escape)){showMenu();}
        if(isPlayingAutomatic && isPaused == false){forward1play();}
    }
    
}
