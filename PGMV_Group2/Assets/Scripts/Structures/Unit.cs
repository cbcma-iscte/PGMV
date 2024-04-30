using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Id { get; set; }
    public string Role { get; set; }
    public string Type { get; set; }
    public string Action { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    private float speed = 1.5f;
    
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
        switch(Role){
            case "catapult":
            break;
            case "mage":
            break;
            case "soldier":
            break;
            case "archer":
            break;
            default:
            break;
        }
        // Attack Logic and Animation
        Debug.Log("Attacking");
    }

    public void Hold()
    {
        // Play Hold Animation
        Debug.Log("Holding");
    }

    public GameObject Spawn(GameObject prefab,Board board, int x, int y)
    {   
        this.prefab = prefab;
        GameObject newObject = Instantiate(prefab, board.FindPositionOfTile(x,y), Quaternion.identity);
        return newObject;
    }

    public void MoveTo(Board board,int x, int y)
    {
        //needs to be different depending on the character
        if(this.Role=="catapult"){
            Hold(); //doesnt move
        }else if(this.Role=="mage"){
        //needs to go up  
        }else{
        transform.position = Vector3.MoveTowards(transform.position,board.FindPositionOfTile(x,y), speed * Time.deltaTime );
        }// Moving Logic and Animation
    }

    public void Die()
    {
        // Die Logic and destroys itself from gamescene (?)
        isDead = true;
        Destroy(gameObject);
    }
}
