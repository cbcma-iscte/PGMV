using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Id { get; set; }
    public string Role { get; set; }
    public string Type { get; set; }
    public string Action { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public bool isDead;

    private GameObject prefab;

    public void Initialize(string id, string role, string type, string action, int x, int y)
    {
        Id = id;
        Role = role;
        Type = type;
        Action = action;
        X = x;
        Y = y;
        isDead = false;
        Debug.Log("Action: "+ action + ", Type: " + type + ", id: "+ id + " at x=" + X + " y=" + Y);
    }

    public void Attack()
    {
        // Attack Logic and Animation
        Debug.Log("Attacking");
    }

    public void Hold()
    {
        // Play Hold Animation
        Debug.Log("Holding");
    }

    public GameObject Spawn(GameObject prefab, Vector3 spawnPosition)
    {
        this.prefab = prefab;
        GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        return newObject;
    }

    public void MoveTo(int x, int y)
    {
        Debug.Log("Moving");
        // Moving Logic and Animation
    }

    public void Die()
    {
        // Die Logic and destroys itself from gamescene (?)
        isDead = true;
        Destroy(gameObject);
    }
}
