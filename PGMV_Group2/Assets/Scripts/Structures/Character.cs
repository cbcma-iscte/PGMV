using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
public class Character : MonoBehaviour
{

    [SerializeField]
    public GameObject weapon;
    [SerializeField]
    public GameObject characterTransparent;
    private GameObject theSmoke;

    [SerializeField]
    public GameObject smokeDeath_blue;
    [SerializeField]
    public GameObject smokeDeath_red;
    private float alpha = 0f;
    private bool smoking = false;
    private string looksSide = "";

    [SerializeField]
    public TrailRenderer trailRenderer;    
    
    
    public IntPair lastPosition;
    private Vector3 finalPosition;

    private bool TrailPainted = false;
    public bool canMove = false;
    private bool isMoving = false;
    public bool isHolding = false;
    public bool isDead;
    public float duration = 5f;
    private float timer = 0f;
    private float speed = 0.5f;
    public string Id;
    public string Role;
    [SerializeField]
    public string Roles_Names;
    
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

    private void Awake(){
        lastPosition = new IntPair(0,0);
    }

    private void Start()
    { 
        lastPosition = new IntPair(0,0);
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
            Color c = Color.white;
            c.a = alpha;
            trailRenderer.material.SetColor("_Color",c);
            
            
            trailRenderer.enabled = true;
        }
        else
        {
            Debug.LogError("No TrailRenderer found on this GameObject.");
        }
    }
    
    public void Initialize(string id, string role,List<Role> roles)
    {
        Id = id;
        Role = role;
        isDead = false;
        Roles_Names = roles[0].Name;
        
        
    }


    public void Hold(){
        isHolding = true;
    }

     
    
    public GameObject Spawn(GameObject prefab,Board board, int x, int y,string role,string id, List<Role> roles)
    {   
        
        lastPosition = new IntPair(x, y); 
        Initialize(id,role,roles);
        GameObject newObject = Instantiate(prefab, board.getBoardByName());
        float positionY = 0.25f;

        if(prefab.tag=="catapult"){
            positionY = 0.112f;
        }

        newObject.transform.localPosition = findPosition(board,positionY,x,y);
       
        string uniqueName = prefab.name + "-" + id;
        
        finalPosition = transform.position;
        newObject.name = uniqueName;
        
        
        Color c = Color.white;
        if(role == roles[0].Name){
            c = Color.blue;
        }else{
            c= Color.red;
        }
        newObject.transform.Find("base").GetComponent<Renderer>().material.color = c;
        isHolding = false;
        
        return newObject;

    }
    
    public void MoveTo(Board board,int x, int y)
    {   

        Transform boardOfCharacter = board.getBoardByName();
        float[] positions = movePositionFloat(board,x,y);
        float posX = positions[0];
        float posZ = positions[1];
        if(this.tag=="catapult"){
            Hold(); 
            canMove = false;
        }else if(this.tag=="mage"){
            canMove = true;  
            isMoving = true;
            finalPosition = new Vector3( transform.localPosition.x + (x - (transform.localPosition.x + posX)),(transform.localPosition.y + 0.5f), transform.localPosition.z + (board.Height - y + 1 - (transform.localPosition.z+posZ)));
            lastPosition = new IntPair(x, y); 
        }else{    
            isHolding = false; 
            canMove = true;  
            isMoving = true;

            finalPosition = new Vector3( transform.localPosition.x + (x - (transform.localPosition.x + posX)),transform.localPosition.y, transform.localPosition.z + (board.Height - y + 1 - (transform.localPosition.z+posZ)));
            lastPosition = new IntPair(x, y); 
        }
        
    }
    
    private float[] movePositionFloat(Board board,int x,int y){
        int i = 0;
        foreach(Transform child in board.getBoardByName()){
            if(child.GetComponent<Character>()!=null){              
                if(isChildInTile(child.localPosition.x , x , child.localPosition.z , y , board.Height)){
                    i++;
                }
            }
        }
        float posX = 0f;
        float posZ = 0f;
        switch (i){
            case 0:
            posX = 0.25f;
            posZ = 0.75f;
            break;

            case 1:
            posX = 0.75f;
            posZ = 0.75f;
            break;

            case 2: 
            posX = 0.25f;
            posZ = 0.25f;
            break;  

            case 3:
            posX = 0.75f;
            posZ = 0.75f;
            break;

            default:
            Debug.LogError("Too Many Characters in Tile");
            break;
        }

        float[] positions = new float[2];
        positions[0] = posX;
        positions[1] = posZ;
        return  positions;
    }
 

    public void Attack(Board board, int x, int y)
    { 
        isHolding = false;
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
    private void soldierAttacks(Board board, int x, int y){
        GameObject charactersWeapon = Instantiate(weapon, transform.localPosition,Quaternion.identity );
        charactersWeapon.transform.SetParent(this.transform);
        charactersWeapon.transform.localPosition = this.transform.localPosition;
        charactersWeapon.GetComponent<Sword>().attack();

        attackCharactersAt(board,x,y);
    }


    private Vector3 findPosition(Board board,float positionY, int x, int y){
        int i = 0;
        Vector3 position = new Vector3();
        foreach(Transform child in board.getBoardByName()){
            if(child.GetComponent<Character>()!=null){              
                if(isChildInTile(child.localPosition.x , x , child.localPosition.z , y , board.Height)){
                    i++;
                }
            }
        }
        switch (i){
            case 0:
            position = new Vector3(x - 0.25f, positionY,(board.Height-y+1) - 0.75f);
            looksSide = "left";
            break;

            case 1:
            position = new Vector3(x - 0.75f, positionY,(board.Height-y+1) - 0.75f) ;
            looksSide = "right";
            break;

            case 2: 
            position = new Vector3(x - 0.25f, positionY,(board.Height-y+1) - 0.25f) ;
            looksSide = "left";
            break;  

            case 3:
            position = new Vector3(x - 0.75f, positionY,(board.Height-y+1) - 0.25f) ;
            looksSide = "left";
            break;

            default:
            Debug.LogError("Too Many Characters in Tile");
            break;
        }
        return position;
    }

    

    

    private void attackCharactersAt(Board b, int x, int y){
       
        List<GameObject> enemies = new List<GameObject>();
        foreach(Transform child in b.getBoardByName()){
            if(child.tag!="Tile" && child.GetComponent<Character>()!=null && child.gameObject.GetComponent<Character>().Role != this.Role){    
                if(isChildInTile(child.localPosition.x , x , child.localPosition.z , y , b.Height)){
                    enemies.Add(child.gameObject);
                }
            }     
        }
        if(enemies.Count!=0){
            foreach(GameObject enemy in enemies){
                if(enemy.transform.tag == tag && tag == "soldier" && enemy.GetComponent<Character>().isHolding){
                    Debug.Log("Is holding So I enter scene!");
                    //enter scene
                }
                enemy.GetComponent<Character>().isDead = true;
            } //need pontuation 
        }
    }

    private bool isChildInTile(float posX ,  int x , float posY , int y, int height){
        if((x-1)<posX && posX<x && (height-y)<posY && posY<(height-y+1)){
            return true;
        }
            
        return false;
    }

    private void viewTrail(){
            if(alpha > 0f){
                alpha = 0f;
            }else{
                alpha = 1f;
            }
            
            if(Role == Roles_Names){
               Color c = Color.blue;
                c.a = alpha; 
            this.trailRenderer.material.SetColor("_Color", c);
            }else{  
                Color c = Color.red;
                c.a = alpha; 
               trailRenderer.material.SetColor("_Color", c);
            }
            Debug.Log("my alpha " + alpha);
            
    }

    private IEnumerator createTransparent(){

        Color c = Color.white;
         if(Role == Roles_Names){
            c = Color.blue;
            c.a = 0.5f;
        }else{
            c= Color.red;
            c.a = 0.5f;
        }   
        
        GameObject itTransparent = Instantiate(characterTransparent, transform.position, transform.rotation);
        itTransparent.transform.Find("base").GetComponent<Renderer>().material.color = c;
        
        yield return new WaitForSeconds(5f);
        Destroy(itTransparent);
    }
    

    void Update(){
        
      
        if (Input.GetMouseButtonDown(0)) {
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(raycast, out hit,Mathf.Infinity) && hit.collider!=null)
            {
                if(hit.collider.gameObject.GetComponent<Character>()!=null){         
                    if (hit.collider.gameObject.GetComponent<Character>().Id == this.Id){
                        viewTrail();
                    }
                }       
            }
        }

        if(Vector3.Distance(transform.localPosition,finalPosition)>0.1f && canMove && isMoving){
            if(tag == "mage" && transform.localPosition.y<0.5f){
                transform.localPosition = Vector3.MoveTowards(transform.localPosition,new Vector3(transform.localPosition.x,0.5f,transform.localPosition.z), speed * Time.deltaTime); 
            }else{
               transform.localPosition = Vector3.MoveTowards(transform.localPosition,finalPosition, speed * Time.deltaTime);     
            }
        }

        if(Vector3.Distance(transform.localPosition,finalPosition)<=0.1f && canMove && isMoving){
            isMoving = false;
            if(tag!="mage"){
                canMove = false;
            }
        }
        
        if( canMove && !isMoving ){
            if(transform.localPosition.y>0.25f){
                transform.localPosition = Vector3.MoveTowards(transform.localPosition,new Vector3(transform.localPosition.x,0.25f,transform.localPosition.z), speed * Time.deltaTime);     
            }
            else{
                canMove = false;
            }
           
        }

        if(isDead && !canMove ){
            if(!smoking){
                smoking = true;
                if(Role == Roles_Names){
                    theSmoke = Instantiate(smokeDeath_blue, transform.position, transform.rotation);
                }else{
                    theSmoke = Instantiate(smokeDeath_red, transform.position, transform.rotation);
                }
                theSmoke.SetActive(true);
                
            }
            
            
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 1f * Time.deltaTime);
            
            if (Vector3.Distance(transform.localScale, Vector3.zero) < 0.2f){
                Destroy(this.gameObject);
                Destroy(theSmoke);
                StartCoroutine(createTransparent());
                isDead = false;
                

            }
        }
        
        
        
    }

   
}