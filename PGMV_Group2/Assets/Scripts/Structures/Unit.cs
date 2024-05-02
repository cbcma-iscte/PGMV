using UnityEngine;



public class Unit : MonoBehaviour {

    public string Id { get; set; }
    public string Role { get; set; }
    public string Action { get; set; }
    public string Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    

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
