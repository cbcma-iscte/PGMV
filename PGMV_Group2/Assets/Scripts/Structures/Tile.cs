using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public string Type { get; set; }
    public Tile(string Type)
    {
        this.Type = Type;
        Debug.Log($"Created tile: {Type}");
    }


 
}

