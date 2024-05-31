using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Character : MonoBehaviour
{

    [SerializeField]
    public GameObject weapon;

    private GameObject theSmoke;

    [SerializeField]
    public GameObject smokeDeath_blue;
    [SerializeField]
    public GameObject smokeDeath_red;
    private float alpha = 0f;
    private bool smoking = false;

    [SerializeField]
    public TrailRenderer trailRenderer;    
    
    [SerializeField]
    public IntPair lastPosition = null;
    private Vector3 finalPosition;
    private bool canMove = false;
    public bool isDead;
    public float duration = 5f;
    private float timer = 0f;
    private float speed = 1.5f;

    public string Id;
    public string Role;
    
    [SerializeField]
    public GameObject prefab;
    

    public class IntPair{
    public int coordX { get; set; }
    public int coordY { get; set; }

    public IntPair(int x, int y)
    {
        coordX = x;
        coordY = y;
    }

    }

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        
        if (trailRenderer != null)
        {
            trailRenderer.time = 99999f;
            
            trailRenderer.startWidth = 0.1f;
            trailRenderer.endWidth = 0.1f;

            Material trailMaterial = new Material(Shader.Find("Standard"));
            
            trailMaterial.SetFloat("_Mode", 3);
            trailMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            trailMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            trailMaterial.SetInt("_ZWrite", 0);
            trailMaterial.DisableKeyword("_ALPHATEST_ON");
            trailMaterial.EnableKeyword("_ALPHABLEND_ON");
            trailMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            trailMaterial.renderQueue = 3000;
            
            trailRenderer.material = trailMaterial;

            if( Role== "player 1"){
                Color c = Color.blue;
                c.a = alpha;
            trailRenderer.material.SetColor("_Color", c);
            }else{
               Color c = Color.red;
                c.a = alpha;
               trailRenderer.material.SetColor("_Color", c);
            }
            
            trailRenderer.enabled = true;
        }
        else
        {
            Debug.LogError("No TrailRenderer found on this GameObject.");
        }
    }
    
    public void Initialize(string id, string role)
    {
        Id = id;
        Role = role;
        isDead = false;
        
    }


    public void Hold(){}

     
    
    public GameObject Spawn(GameObject prefab,Board board, int x, int y,string role,string id)
    {   
        
        lastPosition = new IntPair(x, y); 
        Initialize(id,role);
        GameObject newObject = Instantiate(prefab, board.getBoardByName());
        float positionY = 0.25f;

        if(prefab.tag=="catapult"){
            positionY = 0.112f;
        }

        
        newObject.transform.localPosition = new Vector3(x - 0.5f, positionY,(board.Height-y+1) - 0.5f );
       
        string uniqueName = prefab.name + "-" + id;
    
        finalPosition = transform.position;
        newObject.name = uniqueName;
        
        
        //Debug.Log("this " + this.name + " " + this.lastPosition.coordX + " "+this.lastPosition.coordY);
        Color c = Color.white;
        if(role == "player 1"){
            c = Color.blue;
        }else{
            c= Color.red;
        }
        newObject.transform.Find("base").GetComponent<Renderer>().material.color = c;
        //trailStart();
        
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
            finalPosition = new Vector3( transform.localPosition.x + (x - (transform.localPosition.x + 0.5f)),transform.localPosition.y, transform.localPosition.z + (board.Height - y + 1 - (transform.localPosition.z+0.5f)));
            canMove = true;
            lastPosition = new IntPair(x, y); 
        }
    }
    
    
 

    public void Attack(Board board,int x, int y)
    { 
        switch(this.tag){
            case "catapult": 
                Debug.Log("catapult attacks");
                defaultAttacks(board,x, y);
            break;
            case "mage":
                Debug.Log("mage attacks");
                defaultAttacks(board,x, y);
            break;
            case "soldier":
                soldierAttacks(board,x, y);
            break;
            case "archer":
                Debug.Log("archer attacks");
                defaultAttacks(board,x, y);
            break;
            default:
            break;
        }
    }
    private void defaultAttacks(Board board, int x, int y){
        GameObject charactersWeapon = Instantiate(weapon, transform.localPosition,Quaternion.identity );

        Transform attackingPosition = new GameObject("AttackingPosition").transform;
        attackingPosition.SetParent(board.getBoardByName());

        attackingPosition.localPosition = new Vector3(x - 0.5f, 0.25f,(board.Height-y+1) - 0.5f );

        charactersWeapon.transform.SetParent(board.getBoardByName());
        charactersWeapon.GetComponent<Projectile>().AttackPosition(attackingPosition.localPosition);

        Destroy(attackingPosition.gameObject);
        attackCharactersAt(board,x,y);
    }
    private void soldierAttacks(Board b, int x, int y){
         //change scene to animation (PART2 for SOLDIER ONLY if hold still needs to do)
    }

    

    

    private void attackCharactersAt(Board b, int x, int y){
        Debug.Log("im here");
        List<GameObject> enemies = new List<GameObject>();
        
        foreach(Transform child in b.getBoardByName()){
            if(child.GetComponent<Character>()!=null && lastPosition!=null){
                 Debug.Log("character position: " + child.name + " " + child.GetComponent<Character>().lastPosition.coordX + child.GetComponent<Character>().lastPosition.coordY + "THE X AND Y" + x + y);
                 if(child.GetComponent<Character>().lastPosition.coordX == x && child.GetComponent<Character>().lastPosition.coordY == y){
                    Debug.Log("Child to attack " + child.name);
                    enemies.Add(child.gameObject);
                }
            }
        }
        if(enemies.Count!=0){
            foreach(GameObject enemy in enemies){
                enemy.GetComponent<Character>().isDead = true;
            } //need pontuation 
        }
    }

    public void viewTrail(){
            if(alpha > 0f){
                alpha = 0f;
            }else{
                alpha = 1f;
            }

            if(Role == "player 1"){
               Color c = Color.blue;
                c.a = alpha; 
               trailRenderer.material.SetColor("_Color", c);
            }else{  
                Color c = Color.red;
                c.a = alpha; 
               trailRenderer.material.SetColor("_Color", c);
            }
            Debug.Log("my alpha " + alpha);

    }

    void Update(){

        if(Vector3.Distance(transform.localPosition,finalPosition)>0.1f && canMove){
            transform.localPosition = Vector3.MoveTowards(transform.localPosition,finalPosition, speed * Time.deltaTime);     
        }

        if(Vector3.Distance(transform.localPosition,finalPosition)<=0.1f && canMove){
            canMove = false;
        }

        if(isDead && !canMove ){

            if(!smoking){
                smoking = true;
                if(Role == "player 1"){
                    theSmoke = Instantiate(smokeDeath_blue, transform.position, transform.rotation);
                }else{
                    theSmoke = Instantiate(smokeDeath_red, transform.position, transform.rotation);
                }
                theSmoke.SetActive(true);
                
            }
            
            
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 1f * Time.deltaTime);
            /*alpha = alpha - 0.2f;
            Color color = transform.GetComponent<Renderer>().material.color;
            color.a = alpha;
            transform.GetComponent<Renderer>().material.color = color;
            */

            if (Vector3.Distance(transform.localScale, Vector3.zero) < 0.2f){
                Destroy(this.gameObject);
                Destroy(theSmoke);
                isDead = false;
            }
        }
        
        
        
    }

   
}