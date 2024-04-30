using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{   
    public List<Tile> Tiles;
    private GameObject Table;
    public int Width { get; private set; }
    public int Height { get; private set; }
    private GameObject baseBoard;
    //private List<Tile> Tiles;
    Dictionary<string,Material> TileAndMaterial = new Dictionary<string, Material>();
    private GameObject[,] tilesGenerated; 
    private List<Tile> tiles;

    public void InitializeBoard(int width, int height, Dictionary<string,Material> tile_material, List<Tile> tiles, GameObject table ){
        Debug.Log("Board Board Board is creating!!!");
        Width = width;
        Height = height;
        Table = table;
        TileAndMaterial = tile_material;
        Tiles = tiles;
        createBoard();
        
    }

    public void createBoard(){
        
        baseBoard = new GameObject();
        baseBoard.tag = "Board";
        baseBoard.name = "Board";
        
        createTiles();

        float scaleFactor = 0.9f;
        baseBoard.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        
        float boardWidth = Width * scaleFactor;
        float boardHeight = Height * scaleFactor;
        Vector3 boardCenterInTable = new Vector3(Table.transform.position.x - (boardWidth / 2), Table.transform.position.y + 1.42f, Table.transform.position.z - (boardHeight / 2));
        
        baseBoard.transform.position = boardCenterInTable;
        //Quaternion newRotation = Quaternion.Euler(baseBoard.transform.rotation.eulerAngles.x, baseBoard.transform.rotation.eulerAngles.y, Table.transform.rotation.eulerAngles.z);
        //baseBoard.transform.rotation = newRotation;
        //I cant do the rotation!
      
        }

    private void createTiles(){
        foreach (var kvp in TileAndMaterial)
    {
        Debug.Log("Key: " + kvp.Key + ", Value: " + kvp.Value);
    }
    int i=0;
    tilesGenerated = new GameObject[Width,Height];
        for(int y = Height - 1; y >= 0; y--){
            for(int x=0; x<Width; x++){
                tilesGenerated[x,y] = create1Tile(x,y,Tiles[i].Type);
                i++;
                }
                    
            }
        }

    private GameObject create1Tile(int x , int y,string type){
        GameObject tile_created = new GameObject(string.Format("X{0},Y{1}",x,y)); //X0Y0, X1Y0, X2Y0, X0Y1, X1Y1, X2Y1...
        tile_created.transform.parent = baseBoard.transform; //Associate the tile to the board as its child

        Mesh mesh = new Mesh();
        tile_created.AddComponent<MeshFilter>().mesh = mesh;
        tile_created.AddComponent<MeshRenderer>().material = TileAndMaterial[type]; //the material is working but not appearing like we want


        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(x,0,y); //00
        vertices[1] = new Vector3(x,0,y+1); //01
        vertices[2] = new Vector3(x+1,0,y); //10
        vertices[3] = new Vector3(x+1,0,y+1); //11

        int[] triangles = new int[] { 1, 2, 0, 2, 1, 3 };

        mesh.vertices =vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        tile_created.AddComponent<BoxCollider>();
        

        return tile_created; 

    }
    public Vector3 FindPositionOfTile(int valueX, int valueZ) { //right board, gets the tile correct but i find the spot but, the character doesnt show there.
        string tileName = ("X"+valueX+",Y"+valueZ);
        foreach (GameObject tile in tilesGenerated) {
            if (tile != null && tile.name == tileName) {
                return new Vector3(tile.transform.position.x+valueX, 1.872f, tile.transform.position.z+valueZ);
            }
        }
    return Vector3.zero;
    }
        


    
}
