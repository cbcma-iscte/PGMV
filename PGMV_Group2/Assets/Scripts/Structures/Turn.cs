using System.Collections.Generic;

/// <summary>
/// Represents a turn in the game, containing information about the units involved.
/// </summary>
public class Turn 
{
    /// <summary>
    /// The identifier for the turn.
    /// </summary>
    public int Id;

    /// <summary>
    /// The list of units participating in the turn.
    /// </summary>
    public List<Unit> Units { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the Turn class with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier for the turn.</param>
    public Turn(int id)
    {
        Id = id;
        Units = new List<Unit>();
    }
    
}
