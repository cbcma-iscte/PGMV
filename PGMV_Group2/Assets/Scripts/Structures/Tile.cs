/// <summary>
/// Represents a tile on the game board.
/// </summary>
public class Tile
{
    /// <summary>
    /// The type of the tile.
    /// </summary>
    public string Type { get; set; }

    public int nrOfCharactersInTile = 0;

    /// <summary>
    /// Initializes a new instance of the Tile class with the specified type.
    /// </summary>
    /// <param name="Type">The type of the tile.</param>
    public Tile(string Type)
    {
        this.Type = Type;
    }
}

