using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the game board, including tile creation, character positioning, and score tracking.
/// </summary>
public class Board : MonoBehaviour
{   

    public int pontuationPlayer1 = 0;
    public int pontuationPlayer2 = 0;
    public List<Tile> Tiles;
    private GameObject Table;

    public List<Role> Roles;
    public int Width { get; private set; }
    public int Height { get; private set; }
    private GameObject baseBoard;
    Dictionary<string,Material> TileAndMaterial = new Dictionary<string, Material>();
    private GameObject[,] tilesGenerated; 

    public List<string> battlesInTurn = new List<string>();
    
    /// <summary>
    /// Prevents the board from being destroyed when loading new scenes.
    /// </summary>
    public void Awake(){
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Initializes the board with the specified parameters.
    /// </summary>
    /// <param name="width">Width of the board</param>
    /// <param name="height">Height of the board</param>
    /// <param name="tile_material">Dictionary of tile types and their materials</param>
    /// <param name="tiles">List of tiles to be used</param>
    /// <param name="table">Reference to the game table</param>
    /// <param name="roles">List of roles associated with the board</param>
    public void InitializeBoard(int width, int height, Dictionary<string,Material> tile_material, List<Tile> tiles, GameObject table, List<Role> roles ){
        Width = width;
        Height = height;
        Table = table;
        TileAndMaterial = tile_material;
        Tiles = tiles;
        Roles = roles;
        createBoard();
        
    }

    /// <summary>
    /// Creates the board and initializes the tiles.
    /// </summary>
    public void createBoard(){
        
        baseBoard = new GameObject();
        baseBoard.tag = "Board";
        baseBoard.name = "Board";
        
        createTiles();

        float scaleFactor = 0.9f;
        baseBoard.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        
        float boardWidth = Width * scaleFactor;
        float boardHeight = Height * scaleFactor;
        
        baseBoard.transform.parent = Table.transform;

        baseBoard.transform.localRotation = Quaternion.Euler(new Vector3(0f,0f, Table.transform.localRotation.z));
        
        baseBoard.transform.localPosition = new Vector3(0- (boardWidth / 2),1.415f,0-(boardHeight / 2));
        
        }
    
    /// <summary>
    /// Returns the transform of the base board.
    /// </summary>
    /// <returns>Transform of the base board</returns>
    public Transform getBoardByName(){
        return baseBoard.transform;
    }

    /// <summary>
    /// Creates and initializes the tiles on the board.
    /// </summary>
    private void createTiles(){
    int i=0;
    tilesGenerated = new GameObject[Width,Height];
        for(int y = Height - 1; y >= 0; y--){
            for(int x=0; x<Width; x++){
                tilesGenerated[x,y] = create1Tile(x,y,Height - y,Tiles[i].Type);
                i++;
                }
                    
            }
        }

    /// <summary>
    /// Creates a single tile at the specified position.
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="place">Height place</param>
    /// <param name="type">Type of the tile</param>
    /// <returns>Created tile GameObject</returns>
    private GameObject create1Tile(int x , int y,int place,string type){
        GameObject tile_created = new GameObject(string.Format("X{0}Y{1}", x + 1, place)); //X1.
        tile_created.transform.parent = baseBoard.transform;

        Mesh mesh = new Mesh();
        tile_created.AddComponent<MeshFilter>().mesh = mesh;
        tile_created.AddComponent<MeshRenderer>().material = TileAndMaterial[type];

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(x,0,y); //00
        vertices[1] = new Vector3(x,0,y+1); //01
        vertices[2] = new Vector3(x+1,0,y); //10
        vertices[3] = new Vector3(x+1,0,y+1); //11

        Vector2[] texture_vertices = new Vector2[4]; 

        texture_vertices[0] = new Vector2(0f, 0f); // left bot
        texture_vertices[1] = new Vector2(0f, 1f); // left top
        texture_vertices[2] = new Vector2(1f, 0f); // right bot
        texture_vertices[3] = new Vector2(1f, 1f); // right top

        int[] triangles = new int[] { 1, 2, 0, 2, 1, 3 };

        mesh.vertices =vertices; //vertices
        mesh.uv = texture_vertices; //texture
        mesh.triangles = triangles; //triangles

        mesh.RecalculateNormals();
        tile_created.AddComponent<BoxCollider>();
        
        tile_created.tag = "Tile";
        return tile_created; 

    }

    /// <summary>
    /// Gets the transform of a tile based on its name.
    /// </summary>
    /// <param name="valueX">X coordinate</param>
    /// <param name="valueZ">Z coordinate</param>
    /// <returns>Transform of the tile</returns>
    public Transform getTileFromName(float valueX, float valueZ){
        string tileName = ("X"+ valueX +"Y" + valueZ);
        foreach (GameObject tile in tilesGenerated) {
            if (tile != null && tile.name == tileName) {
                return tile.transform;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Finds a character on the board based on the specified unit.
    /// </summary>
    /// <param name="unit">Unit to find</param>
    /// <returns>GameObject of the found character, or null if not found</returns>
    public GameObject findCharacterInBoard(Unit unit){
        foreach(Transform child in baseBoard.transform){
            if(child.GetComponent<Character>() != null){
                Character character = child.GetComponent<Character>();
                if(character.Role == unit.Role && character.Id == unit.Id && child.tag == unit.Type)
                    return child.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Adds a point to the specified player's score.
    /// </summary>
    /// <param name="nameOfPlayer">Name of the player</param>
    public void addPointTo(string nameOfPlayer){
        if(nameOfPlayer==Roles[0].Name){
            pontuationPlayer1 ++;
            
        }else{
             pontuationPlayer2 ++;
            
        }

        
       
    }

    /// <summary>
    /// Resets the scores of both players to zero.
    /// </summary>
    public void restartPontuation(){
        pontuationPlayer1 =0;
        pontuationPlayer2 =0;
    }

    /// <summary>
    /// Gets the material of a tile based on its coordinates.
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <returns>Name of the material</returns>
    public string getMaterial(int x, int y){
        Transform tile = getTileFromName(x,y);
        return tile.GetComponent<MeshRenderer>().material.name;
    }

    /// <summary>
    /// Changes the scores of both players.
    /// </summary>
    /// <param name="p1">New score for player 1</param>
    /// <param name="p2">New score for player 2</param>
    public void changePontuation(int p1, int p2){
        pontuationPlayer1 =p1;
        pontuationPlayer2 =p2;
    }

    /// <summary>
    /// Adds a battle to the current turn based on the tile material.
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public void addBattle(int x,int y){
        battlesInTurn.Add(getMaterial(x, y));
    }

    /// <summary>
    /// Clears the list of battles for the current turn.
    /// </summary>
    public void battlesDelivered(){
        battlesInTurn.Clear();
    }

   
    
    
}
