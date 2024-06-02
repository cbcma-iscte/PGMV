using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    private bool isPlayingAutomatic = false;
    // we give option for automatic game or manual game -> play goes automatic, pause goes manual with the forward and back Turns
    public GameObject[] Games;
    private int currentValuePlayer1 = 0;
    private int currentValuePlayer2 = 0;

    [SerializeField] public TextMeshProUGUI player1Points;
    
    [SerializeField] public TextMeshProUGUI player2Points;
    
    public void Start(){
       Games = GameObject.FindGameObjectsWithTag("GameController");
    }
    public void start_pause_Game(){
        foreach(GameObject game in Games)
        {
            if(isPlayingAutomatic){
                game.GetComponent<GameManager>().PauseResumeGame();
        
            }else{
                game.GetComponent<GameManager>().PauseResumeGame(); 
            }
            isPlayingAutomatic=!isPlayingAutomatic;
        }
        
    }

    public void restart(){
        foreach(GameObject game in Games)
        {
            game.GetComponent<GameManager>().RestartGame();
        }
    }

    public void back1play(){

        //see the previous play
        foreach(GameObject game in Games)
        {
            game.GetComponent<GameManager>().GoBack();
        }

    }

    public void forward1play(){
        //see the next turn
        foreach(GameObject game in Games)
        {
            game.GetComponent<GameManager>().GoForward();
        }
    }

    public void killedEnemy(string player){
        if(player.Equals("player1")){
            player1Points.text = " " +(currentValuePlayer1 + 1).ToString() +" points";
            currentValuePlayer1++;
        }
        else{
            player2Points.text = " "+(currentValuePlayer2 + 1).ToString() +" points";
            currentValuePlayer2++;
        }
    }

    void Update(){
        
         if (Input.GetKeyDown(KeyCode.P))
        {
            start_pause_Game();
        }
    }
    
}
