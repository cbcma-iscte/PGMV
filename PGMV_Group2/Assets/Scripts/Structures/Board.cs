using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{   
    public List<Tile> Tiles { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    [SerializeField] private Dictionary<string, Material> TilesMaterials; // Different types of tiles available -> squares
    [SerializeField] private GameObject boardBase; // Reference to the base GameObject

    public void InitializeBoard(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new List<Tile>();

        // Calculate the size of each tile based on the size of the board base
        Vector3 tileSize = new Vector3(boardBase.transform.localScale.x / Width, 1, boardBase.transform.localScale.z / Height);

        // Instantiate tiles based on width and height
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Calculate position for the tile
                Vector3 tilePosition = boardBase.transform.position + new Vector3(tileSize.x * x + tileSize.x / 2, 0, tileSize.z * y + tileSize.z / 2);

                // Instantiate a primitive plane GameObject for the tile
                GameObject tileGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
                tileGO.transform.parent = transform; // Set parent to the board
                tileGO.transform.localPosition = tilePosition;

                // Set material for the tile
                string tileType = "village"/* logic to determine tile type based on position, or any other criteria */;
                if (TilesMaterials.ContainsKey(tileType))
                {
                    Renderer renderer = tileGO.GetComponent<Renderer>();
                    renderer.material = TilesMaterials[tileType];
                }
                else
                {
                    Debug.LogWarning("Material for tile type " + tileType + " not found!");
                }

                // Set tag based on tile type
                tileGO.tag = tileType;

                // Add the tile to the list
                Tiles.Add(new Tile(tileType));
            }
        }
    }
}
