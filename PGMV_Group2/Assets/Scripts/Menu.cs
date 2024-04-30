using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    private bool isPlayingAutomatic = false;
    // we give option for automatic game or manual game -> play goes automatic, pause goes manual with the forward and back Turns
    private GameObject[] Games;
    private int currentValuePlayer1 = 0;
    private int currentValuePlayer2 = 0;

    [SerializeField] public TextMeshProUGUI player1Points;
    
    [SerializeField] public TextMeshProUGUI player2Points;
    public void Start(){
       Games = GameObject.FindGameObjectsWithTag("GameController");
    }
    public void start_pause_Game(){
        if(isPlayingAutomatic){

       
        }else{
            
            
        }
        isPlayingAutomatic=!isPlayingAutomatic;
        
    }

    public void restart(){
        //restart game
    }

    public void return1play(){

        //see the previous play
    }

    public void forward1play(){
        //see the next turn
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

    
}
