using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{   
    public List<Tile> Tiles;
    private GameObject Table;
    public int Width { get; private set; }
    public int Height { get; private set; }
    private GameObject baseBoard;
    Dictionary<string,Material> TileAndMaterial = new Dictionary<string, Material>();
    private GameObject[,] tilesGenerated; 
    

    public void InitializeBoard(int width, int height, Dictionary<string,Material> tile_material, List<Tile> tiles, GameObject table ){
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
        
        baseBoard.transform.parent = Table.transform;

        baseBoard.transform.localRotation = Quaternion.Euler(new Vector3(0f,0f, Table.transform.localRotation.z));
        
        baseBoard.transform.localPosition = new Vector3(0- (boardWidth / 2),1.415f,0-(boardHeight / 2));
        
        }
    
    public Transform getBoardByName(){
        return baseBoard.transform;
    }

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

    private GameObject create1Tile(int x , int y,int place,string type){ //question with teacher (nomenclature)
        GameObject tile_created = new GameObject(string.Format("X{0}Y{1}", x + 1, place)); //X1.
        tile_created.transform.parent = baseBoard.transform;
        // Debug.Log("base board transform do crete1tile : " + baseBoard.transform); 

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

    public Transform getTileFromName(float valueX, float valueZ){
        string tileName = ("X"+ valueX +"Y" + valueZ);
        foreach (GameObject tile in tilesGenerated) {
            if (tile != null && tile.name == tileName) {
                Debug.Log("Name looking: " + tileName + "I am here:" + tile.transform.position);
                return tile.transform;
            }
        }
        return null;
    }
    
    public GameObject findCharacterInBoard(Unit unit){
        foreach(Transform child in baseBoard.transform){
            if(child.GetComponent<Character>() != null){
                Character character = child.GetComponent<Character>();
                if(character.Role == unit.Role && character.Id == unit.Id && child.tag == unit.Type)
                    return child.gameObject;
            }
        }
        Debug.Log("not found");
        return null;
    }

    

    
}
