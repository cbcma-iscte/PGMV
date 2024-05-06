using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Character : MonoBehaviour
{
    [SerializeField]
    public GameObject weapon;

    [SerializeField]
    public string Id;
    [SerializeField]
    public string Role;
    private float speed = 1.5f;

    public GameObject prefab;
    public bool isDead;
    public float duration = 5f;
    private float timer = 0f;

    public GameObject[] trail; //for the trail
    
    public void Initialize(string id, string role)
    {
        Id = id;
        Role = role;
        isDead = false;
    }


    public void Hold(){
    // Play Hold Animation
    }

    public void MoveTo(Board board,int x, int y)
    {/*soldier and archer move throught ground, mage flies, catapult doesnt move*/

        if(this.tag=="catapult"){
            Hold(); 
        }else if(this.tag=="mage"){
        //needs to go up  
        }else{
            transform.position = Vector3.MoveTowards(transform.position,board.FindPositionOfTile(x,y), speed * Time.deltaTime );
        }// Moving Logic and Animation
    }
    
    public GameObject Spawn(GameObject prefab,Board board, int x, int y,string role,string id)
    {   
        this.prefab = prefab;
        GameObject newObject = Instantiate(prefab, board.FindPositionOfTile(x,y), Quaternion.identity);
        
        string uniqueName = prefab.name + "-" + id;

        newObject.name = uniqueName;

        newObject.transform.parent = board.getTileFromName(x,y);
        Initialize(id,role);
       // Debug.Log("My tag is "+this.tag);
       // Debug.Log("My ID is "+this.Id);
        return newObject;

    }

    public void Attack(Board board,int x, int y)
    {
        switch(this.tag){
            case "catapult": 
                catapultAttacks(board,x, y);
            break;
            case "mage":
                mageAttacks(board,x, y);
            break;
            case "soldier":
                soldierAttacks(board,x, y);
            break;
            case "archer":
                archerAttacks(board,x, y);
            break;
            default:
            break;
        }
        // Attack Logic and Animation
        //Debug.Log("Attacking");
    }
    private void catapultAttacks(Board b, int x, int y){
        //throw rock;
        GameObject rock = Instantiate(this.weapon, this.transform.position,Quaternion.identity );//position needs to be seen and stuff
        rock.transform.position = Vector3.MoveTowards(rock.transform.position,b.FindPositionOfTile(x,y), 1f * Time.deltaTime );
        Destroy(rock);
        attackCharactersAt(b,x,y);
        //the catapult throws rocks to its tile or other
    }
    private void mageAttacks(Board b, int x, int y){
        // mage throws fireballs to its tile or others
        GameObject fireball = Instantiate(this.weapon, this.transform.position,Quaternion.identity );
        fireball.transform.position = Vector3.MoveTowards(fireball.transform.position,b.FindPositionOfTile(x,y), 1f * Time.deltaTime );
        Destroy(fireball);
        attackCharactersAt(b,x,y);
    }

    private void archerAttacks(Board b, int x, int y){
        GameObject arrow = Instantiate(this.weapon, this.transform.position,Quaternion.identity );
        arrow.transform.position = Vector3.MoveTowards(arrow.transform.position,b.FindPositionOfTile(x,y), 1f * Time.deltaTime );
        Destroy(arrow);
        //the archer throws arrows to its tile or other
         //throw arrow;
        attackCharactersAt(b,x,y);
    }
    private void soldierAttacks(Board b, int x, int y){
         //attack with sword;
        GameObject sword = Instantiate(this.weapon, this.transform.position,Quaternion.identity);
        Quaternion target = Quaternion.Euler(90f, 0f, 0f);
        sword.transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1f);
        attackCharactersAt(b,x,y);
        Destroy(sword);
        //Soldier attacks with the sword in its own tile
    }

    private void attackCharactersAt(Board b, int x, int y){
        Transform tile = b.getTileFromName(x,y);
        List<GameObject> enemies = new List<GameObject>();
        foreach(GameObject childGameObject in tile){
            if(childGameObject.GetComponent<Character>().Role != this.Role){
                enemies.Add(childGameObject);
            }
        }
        foreach(GameObject enemy in enemies){
            enemy.GetComponent<Character>().Die();
        } //need pontuation
    }

    
    public void Die()
    {   //sound + particles
        //still needed
        //opacity + scale 
        StartCoroutine(GetScaleAndOpacityToZero());
        //logic
        isDead = true;
        //Destroy
        Destroy(gameObject);
    }
    private IEnumerator GetScaleAndOpacityToZero(){
    while (timer < duration)
        {
           
            timer += Time.deltaTime;
            float time = Mathf.Clamp01(timer / duration); // Normalize time between 0 and 1

            Color initialColor = GetComponent<Renderer>().material.color;
            float initialAlpha = initialColor.a;
            Color newColor = initialColor;

            newColor.a = Mathf.Lerp(initialAlpha, 0, time);
            GetComponent<Renderer>().material.color = newColor;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, time);           

            yield return null; // Wait for the next frame
        }
        //make sure it's truly 0
        transform.localScale = Vector3.zero;
    }
}
