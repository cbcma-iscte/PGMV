using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    public string Id { get; set; }
    public string Role { get; set; }
    private float speed = 1.5f;

    public GameObject prefab;
    
    public bool isDead;


    public void Initialize(string id, string role)
    {
        Id = id;
        Role = role;
        isDead = false;
    }

    public void Attack()
    {
        switch(Role){/*Soldier attacks with the sword in its own tile, the archer throws arrows to its tile or others, 
        mage throws fireballs to its tile or others, the catapult throws rocks to its tile or other*/
        
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

    public void MoveTo(Board board,int x, int y)
    {/*soldier and archer move throught ground, mage flies, catapult doesnt move*/

        if(this.Role=="catapult"){
            Hold(); //doesnt move
        }else if(this.Role=="mage"){
        //needs to go up  
        }else{
       // transform.position = Vector3.MoveTowards(transform.position,board.FindPositionOfTile(x,y), speed * Time.deltaTime );
        }// Moving Logic and Animation
    }
    
    public GameObject Spawn(GameObject prefab,Board board, int x, int y)
    {   
        this.prefab = prefab;
        GameObject newObject = Instantiate(prefab, board.FindPositionOfTile(x,y), Quaternion.identity);
        newObject.transform.parent = board.getTileFromName(x,y);
        return newObject;
    }
    public void Die()
    {
        // Die Logic and destroys itself from gamescene (?)
        isDead = true;
        Destroy(gameObject);
    }
}
