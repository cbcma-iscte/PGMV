using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public string Type { get; set; }

    public int nrOfCharactersInTile = 0;
    public Tile(string Type)
    {
        this.Type = Type;
    }

    public void addCharactertoTile(){
        nrOfCharactersInTile++;
    }

     public void removeCharactertoTile(){
        nrOfCharactersInTile--;
    }

 
}

