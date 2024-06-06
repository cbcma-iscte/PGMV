using UnityEngine;


/// <summary>
/// This class is used to store the information of a unit.
/// </summary>
public class Unit : MonoBehaviour {

    /// <summary>
    /// The id of the unit.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The role of the unit.
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// The action performed by the unit.
    /// </summary>  
    public string Action { get; set; }

    /// <summary>
    /// The type of the unit.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The x coordinate of the unit.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// The y coordinate of the unit.
    /// </summary>
    public int Y { get; set; }
    
    /// <summary>
    /// The constructor of the class.
    /// </summary>
    /// <param name="id">The id of the unit.</param>
    /// <param name="role">The role of the unit.</param>
    /// <param name="type">The type of the unit.</param>
    /// <param name="x">The x coordinate of the unit.</param>
    /// <param name="y">The y coordinate of the unit.</param>
    /// <param name="action">The action performed by the unit.</param>
    public Unit(string id, string role, string type, int x, int y, string action)
    {
        Action = action;
        Id = id;
        Role = role;
        Type = type;
        X = x;
        Y = y;
    }

   
}
