using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{   
    public List<Tile> Tiles;
    public int Width { get; private set; }
    public int Height { get; private set; }
    private GameObject baseBoard;
    private GameObject[] Tables_Boards_ToLook;
    [SerializeField] private Dictionary<string, Material> TilesMaterials; // Different types of tiles available -> squares

    public void InitializeBoard(int width, int height){
        Debug.Log("Board Board Board is creating!!!");
        Width = width;
        Height = height;
        createBoard();
        
    }

    public void createBoard(){
                baseBoard = GameObject.CreatePrimitive(PrimitiveType.Cube);
                baseBoard.tag = "Board";
        
        }
        


    
}
