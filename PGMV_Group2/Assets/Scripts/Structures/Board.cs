using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    
    public List<Tile> Tiles { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public void InitializeBoard(int width, int height, List<Tile> tiles){
        Width = width;
        Height = height;
        Tiles = tiles;
    }
}
