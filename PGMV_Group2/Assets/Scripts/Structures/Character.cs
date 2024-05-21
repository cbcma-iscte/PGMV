using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Character : MonoBehaviour
{
    [SerializeField]
    public GameObject weapon;

    public IntPair lastPosition;
    private Vector3 finalPosition;
    public string Id;
    public string Role;
    private float speed = 1.5f;
    [SerializeField]
    public GameObject prefab;
    public bool isDead;
    public bool canMove = false;
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
        Initialize(id,role);
        Debug.Log("x : " + x);
        GameObject newObject = Instantiate(prefab, new Vector3(board.getBoardByName().position.x+x, board.getBoardByName().position.y + 0.5f , board.getBoardByName().position.z+y), Quaternion.identity);
        string uniqueName = prefab.name + "-" + id;
        newObject.transform.SetParent(board.getBoardByName(),true);
        finalPosition = transform.position;
        newObject.name = uniqueName;
        IntPair newPair = new IntPair(x, y); 
        trail.Add(newPair);
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
            IntPair newPair = new IntPair(x, y); // Example values
            finalPosition = new Vector3(transform.localPosition.x+x,transform.localPosition.y,transform.localPosition.z+y);
            canMove = true;
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
        //GameObject charactersWeapon = Instantiate(this.weapon, this.transform.position,Quaternion.identity );
        //charactersWeapon.transform.position = Vector3.MoveTowards(charactersWeapon.transform.position,b.FindPositionOfTile(x,y), 1f * Time.deltaTime );
        //Destroy(charactersWeapon);
        //the archer throws arrows to its tile or other and the mage the fireball
        attackCharactersAt(b,x,y);
    }
    private void soldierAttacks(Board b, int x, int y){
         //attack with sword;
        //GameObject sword = Instantiate(this.weapon, this.transform.position,Quaternion.identity);
        //Quaternion target = Quaternion.Euler(90f, 0f, 0f);
        //sword.transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1f);
        //attackCharactersAt(b,x,y);
        //Destroy(sword);
        //Soldier attacks with the sword in its own tile
    }

    private List<GameObject> GetCharactersAt(Board b, int x, int y)
    {
        List<GameObject> characters = new List<GameObject>();
        foreach(Transform child in b.getBoardByName()){
            if(child.GetComponent<Character>() != null && child.GetComponent<Character>().trail.Count > 0){
                List<IntPair> charTrail = child.GetComponent<Character>().trail;
                IntPair lastTrail = charTrail[charTrail.Count - 1];
                if(child.GetComponent<Character>().Role != this.Role && lastTrail.coordX == x && lastTrail.coordY == y){
                    characters.Add(child.gameObject);
                }
            }
        }
        return characters;
    }

    private int GetNrCharacters(Board b, int x, int y)
    {
        if ( GetCharactersAt(b, x, y).Count < 1)
            return 1;
        else 
            return GetCharactersAt(b, x, y).Count;
    }

    private void attackCharactersAt(Board b, int x, int y){
        List<GameObject> enemies = new List<GameObject>();
        foreach(Transform child in b.getBoardByName()){
            if(child.GetComponent<Character>() != null && child.GetComponent<Character>().trail.Count > 0){
                List<IntPair> charTrail = child.GetComponent<Character>().trail;
                IntPair lastTrail = charTrail[charTrail.Count - 1];
                if(child.GetComponent<Character>().Role != this.Role && lastTrail.coordX == x && lastTrail.coordY == y){
                    enemies.Add(child.gameObject);
                    Debug.Log( "added enemy " + child.GetComponent<Character>().Id);
                    Debug.Log( "x,y : " + lastTrail.coordX + ", " + lastTrail.coordY);
                }
            }
        }
        if(enemies.Count!=0){
            foreach(GameObject enemy in enemies){
                Debug.Log(" dying");
                enemy.GetComponent<Character>().Die();
            } //need pontuation and verify end of turn doesnt have a character left in that spot
        }
    }

    
    public void Die()
    {   //sound + particles
        //still needed
        //opacity + scale not done the scale
        StartCoroutine(GetScaleAndOpacityToZero());
        //logic
        isDead = true;
        //Destroy
        Destroy(this.gameObject);
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

    public void Update(){
        if(transform.position!=finalPosition && canMove){
            Debug.Log("final position : " + finalPosition);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition,finalPosition, speed * Time.deltaTime);     
        }
        canMove = false;
    }
}
