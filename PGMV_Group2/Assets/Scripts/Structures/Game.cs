using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public Role[] roles;
    public Board board;
    public Turn[] turns;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void BuildGame(Role[] roles, Board board, Turn[] turns)
    {
        this.roles = roles;
        this.board = board;
        this.turns = turns;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
