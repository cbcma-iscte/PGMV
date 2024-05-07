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

    public class IntPair{
    public int coordX { get; set; }
    public int coordY { get; set; }

    public IntPair(int x, int y)
    {
        coordX = x;
        coordY = y;
    }

    }

    public List<IntPair> trail = new List<IntPair>();
    public void Initialize(string id, string role)
    {
        Id = id;
        Role = role;
        isDead = false;
    }


    public void Hold(){}


    
    public GameObject Spawn(GameObject prefab,Board board, int x, int y,string role,string id)
    {   
        this.prefab = prefab;
        
        GameObject newObject = Instantiate(prefab, new Vector3(board.getBoardByName().position.x, board.getBoardByName().position.y+0.5f, board.getBoardByName().position.z), Quaternion.identity);
        string uniqueName = prefab.name + "-" + id;
        newObject.transform.SetParent(board.getBoardByName());
        
        newObject.name = uniqueName;
        //IntPair newPair = new IntPair(x, y); 
        //trail.Add(newPair);
        Initialize(id,role);
        return newObject;

    }
    
    public void MoveTo(Board board,int x, int y)
    {
        Transform boardOfCharacter = board.getBoardByName();
        /*soldier and archer move throught ground, mage flies, catapult doesnt move*/
        if(this.tag=="catapult"){
            Hold(); 
        }else if(this.tag=="mage"){
        //needs to go up  
        }else{
            transform.position = Vector3.MoveTowards(transform.position,new Vector3(transform.position.x+x,transform.position.y,transform.position.z+y), speed * Time.deltaTime);     
            IntPair newPair = new IntPair(x, y); // Example values
            trail.Add(newPair);
        }
    }
    
    public void Attack(Board board,int x, int y)
    {
        switch(this.tag){
            case "catapult": 
                defaultAttacks(board,x, y);
            break;
            case "mage":
                defaultAttacks(board,x, y);
            break;
            case "soldier":
                soldierAttacks(board,x, y);
            break;
            case "archer":
                defaultAttacks(board,x, y);
            break;
            default:
            break;
        }
        // Attack Logic and Animation (PART2 for SOLDIER ONLY)
    }
    private void defaultAttacks(Board b, int x, int y){
        GameObject charactersWeapon = Instantiate(this.weapon, this.transform.position,Quaternion.identity );
        charactersWeapon.transform.position = Vector3.MoveTowards(charactersWeapon.transform.position,b.FindPositionOfTile(x,y), 1f * Time.deltaTime );
        Destroy(charactersWeapon);
        //the archer throws arrows to its tile or other and the mage the fireball
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
        foreach(Transform child in tile){
            if(child.GetComponent<Character>().Role != this.Role){
                enemies.Add(child.gameObject);
            }
        }
        foreach(GameObject enemy in enemies){
            enemy.GetComponent<Character>().Die();
        } //need pontuation and verify end of turn doesnt have a character left in that spot

    }

    
    public void Die()
    {   //sound + particles
        //still needed
        //opacity + scale not done the scale
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

            //Color initialColor = GetComponent<Renderer>().material.color;
            //float initialAlpha = initialColor.a;
            //Color newColor = initialColor;

            //newColor.a = Mathf.Lerp(initialAlpha, 0, time);
            //GetComponent<Renderer>().material.color = newColor;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, time);           

            yield return null; // Wait for the next frame
        }
        //make sure it's truly 0
        transform.localScale = Vector3.zero;
    }
}
