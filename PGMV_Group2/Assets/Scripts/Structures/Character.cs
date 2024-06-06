using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Character class represents a character in the game, with functionalities for movement, spawning, attacking, and dying.
/// </summary>
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

    [SerializeField]
    public TrailRenderer trailRenderer;    
    public IntPair lastPosition;
    private Vector3 finalPosition;
    public bool canMove = false;
    private bool isMoving = false;
    public bool isHolding = false;
    public bool isDead = false;
    public float duration = 5f;
    private float speed = 0.5f;
    public string Id;
    public string Role;
    [SerializeField]
    public string Roles_Names;
    
    [SerializeField]
    public GameObject prefab;

    
    /// <summary>
    /// IntPair class to hold coordinates.
    /// </summary>
    public class IntPair{
    public int coordX { get; set; }
    public int coordY { get; set; }

    public IntPair(int x, int y)
    {
        coordX = x;
        coordY = y;
    }

    }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake(){
        DontDestroyOnLoad(gameObject);
        lastPosition = new IntPair(0,0);
    }

    /// <summary>
    /// Initializes the TrailRenderer with the desired settings..
    /// </summary>
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
    
    /// <summary>
    /// Initializes the Character with the desired properties.
    /// </summary>
    /// <param name="id">Character ID.</param>
    /// <param name="role">Character role.</param>
    /// <param name="roles">List of roles.</param>
    public void Initialize(string id, string role,List<Role> roles)
    {
        Id = id;
        Role = role;
        isDead = false;
        Roles_Names = roles[0].Name;
        
        
    }

    /// <summary>
    /// Puts the character in a holding state.
    /// </summary>
    public void Hold(){
        isHolding = true;
    }

     
    /// <summary>
    /// Spawns a new character at the specified position.
    /// </summary>
    /// <param name="prefab">Prefab of the character.</param>
    /// <param name="board">Board object.</param>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="role">Role of the character.</param>
    /// <param name="id">ID of the character.</param>
    /// <param name="roles">List of roles.</param>
    /// <returns>Spawned GameObject.</returns>
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
    
    /// <summary>
    /// Moves the character to the specified position.
    /// </summary>
    /// <param name="board">Board object.</param>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
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
    
    /// <summary>
    /// Calculates the position offset for movement.
    /// </summary>
    /// <param name="board">Board object.</param>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>Array of position offsets [posX, posZ].</returns>
    private float[] movePositionFloat(Board board,int x,int y){
        List <Transform> childsInTile = new List<Transform>();

        foreach(Transform child in board.getBoardByName()){
            if(child.GetComponent<Character>()!=null){              
                if(isChildInTile(child.localPosition.x , x , child.localPosition.z , y , board.Height)){
                    childsInTile.Add(child);
                }
            }
        }
        
        int i = 0;
        if(childsInTile.Count>0) i= findEmptyPosition(childsInTile,x,y,board);

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
    
    /// <summary>
    /// Scatters characters in tile proportionally.
    /// </summary>
    /// <param name="board">Board object.</param>
    /// <param name="positionY">Y position.</param>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>position for character.</returns>
    private Vector3 findPosition(Board board,float positionY, int x, int y){
        List <Transform> childsInTile = new List<Transform>();
        Vector3 position = new Vector3();
        foreach(Transform child in board.getBoardByName()){
            if(child.GetComponent<Character>()!=null){              
                if(isChildInTile(child.localPosition.x , x , child.localPosition.z , y , board.Height)){
                    childsInTile.Add(child);
                }
            }
        }
        int i = 0;
        if(childsInTile.Count>0) i= findEmptyPosition(childsInTile,x,y,board);
        switch (i){
            case 0:
            position = new Vector3(x - 0.25f, positionY,(board.Height-y+1) - 0.75f);
            
            break;

            case 1:
            position = new Vector3(x - 0.75f, positionY,(board.Height-y+1) - 0.75f) ;
            
            break;

            case 2: 
            position = new Vector3(x - 0.25f, positionY,(board.Height-y+1) - 0.25f) ;
            
            break;  

            case 3:
            position = new Vector3(x - 0.75f, positionY,(board.Height-y+1) - 0.25f) ;
            
            break;

            default:
            Debug.LogError("Too Many Characters in Tile");
            break;
        }
        return position;
    }
    /// <summary>
    /// Checks if a child object is in the specified tile.
    /// </summary>
    /// <param name="posX">X position of the child object.</param>
    /// <param name="x">X coordinate of the tile.</param>
    /// <param name="posY">Y position of the child object.</param>
    /// <param name="y">Y coordinate of the tile.</param>
    /// <param name="height">Height of the board.</param>
    /// <returns>Boolean indicating whether the child is in the tile.</returns
    private bool isChildInTile(float posX ,  int x , float posY , int y, int height){
        if((x-1)<posX && posX<x && (height-y)<posY && posY<(height-y+1)){
            return true;
        }
            
        return false;
    }

    /// <summary>
    /// Finds an empty position for a character.
    /// </summary>
    /// <param name="childsInTile">List of child objects in the tile.</param>
    /// <param name="x">X coordinate of the tile.</param>
    /// <param name="y">Y coordinate of the tile.</param>
    /// <param name="board">Board object.</param>
    /// <returns>Index of the empty position.</returns>
    private int findEmptyPosition(List<Transform> childsInTile,int x, int y,Board board){
        List<int> usedPositions = new List<int>(); //0 1 2 and 3 are the possible positions
        int tileFull = -1;
        foreach(Transform child in childsInTile){
            if(child.localPosition.x>(x-0.5f) && child.localPosition.z<((board.Height + 1 - y)-0.5f)){
                usedPositions.Add(0);
            }else if(child.localPosition.x<(x-0.5f) && child.localPosition.z>((board.Height + 1 - y)-0.5f)){
                usedPositions.Add(1);
            }else if(child.localPosition.x>(x-0.5f) && child.localPosition.z<((board.Height + 1 - y)-0.5f)){
                usedPositions.Add(2);
            }else{
                usedPositions.Add(3);
            }
        }

        
        for (int i = 0; i < 4; i++){
        if (!usedPositions.Contains(i)){
            return i;
        }
        }
        
        return tileFull;
    }
    
    /// <summary>
    /// Attacks the specified position on the board.
    /// </summary>
    /// <param name="board">Board object.</param>
    /// <param name="x">X coordinate of the target position.</param>
    /// <param name="y">Y coordinate of the target position.</param>
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

    /// <summary>
    /// Performs the default attack for characters like mage, archer, and catapult.
    /// </summary>
    /// <param name="board">Board object.</param>
    /// <param name="x">X coordinate of the target position.</param>
    /// <param name="y">Y coordinate of the target position.</param>
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

    /// <summary>
    /// Performs the attack for the soldier character.
    /// </summary>
    /// <param name="board">Board object.</param>
    /// <param name="x">X coordinate of the target position.</param>
    /// <param name="y">Y coordinate of the target position.</param>
    private void soldierAttacks(Board board, int x, int y){
        // Instantiate the soldier's weapon
        GameObject charactersWeapon = Instantiate(weapon, transform.localPosition,Quaternion.identity );
        charactersWeapon.transform.SetParent(transform);

        charactersWeapon.transform.localPosition =new Vector3(-0.200000003f,0.170000002f,0.279000014f);
        charactersWeapon.transform.rotation =  Quaternion.Euler(305.502136f,207.265305f,252.824036f);
        charactersWeapon.GetComponent<Sword>().attack();

        // Attack other characters in the target position
        attackCharactersAt(board,x,y);
    }

    /// <summary>
    /// Attacks characters at the specified position on the board.
    /// </summary>
    /// <param name="board">Board object.</param>
    /// <param name="x">X coordinate of the target position.</param>
    /// <param name="y">Y coordinate of the target position.</param>
    private void attackCharactersAt(Board b, int x, int y){

        List<GameObject> enemies = new List<GameObject>();
        foreach(Transform child in b.getBoardByName()){
            if(child.tag!="Tile" && child.GetComponent<Character>()!=null && child.gameObject.GetComponent<Character>().Role != this.Role){
                if(isChildInTile(child.localPosition.x , x , child.localPosition.z , y , b.Height)){
                    enemies.Add(child.gameObject);
                }
            }
        }
        // Ensure that each enemy is only attacked once
        enemies = makeSureNoRepete(enemies);
            // Attack enemies and handle game logic
            if(enemies.Count>0){
            foreach(GameObject enemy in enemies){
                b.addPointTo(Role);
                enemy.GetComponent<Character>().isDead = true;
                // Trigger battle if both attacker and target are soldiers and the target is holding
                if(enemy.transform.tag == tag && tag == "soldier" && enemy.GetComponent<Character>().isHolding){
                    b.addBattle(x,y);
                }
            } 
        }

    }

    /// <summary>
    /// Ensures that each enemy is attacked only once by removing duplicates.
    /// </summary>
    /// <param name="enemies">List of enemy game objects.</param>
    /// <returns>List of unique enemy game objects.</returns>
    private List<GameObject> makeSureNoRepete(List<GameObject> enemies){
         HashSet<string> uniqueIds = new HashSet<string>();
        List<GameObject> uniqueEnemies = new List<GameObject>();

        foreach (GameObject enemy in enemies)
        {
            Character characterComponent = enemy.GetComponent<Character>();
            if (characterComponent != null && uniqueIds.Add(characterComponent.Id ))
            {
                uniqueEnemies.Add(enemy);
            }else{
                Destroy(enemy);
            }
        }

        return uniqueEnemies;
    }

    /// <summary>
    /// Toggles the visibility of the trail renderer of the character.
    /// </summary>
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
            
    }

    /// <summary>
    /// Creates a transparent ghost version of the character when it dies, indicating its previous location on the game board.
    /// </summary>
    private void createTransparent(){

        Color c = Color.white;
         if(Role == Roles_Names){
            c = Color.blue;
            c.a = 0.35f;
        }else{
            c= Color.red;
            c.a = 0.35f;
        }   
        
        GameObject itTransparent = Instantiate(characterTransparent, transform.position, transform.rotation);
        if(tag!="catapult"){
            itTransparent.transform.position = new Vector3(itTransparent.transform.position.x,1.674f,itTransparent.transform.position.z);
        }
        itTransparent.transform.Find("base").GetComponent<Renderer>().material.color = c;
        itTransparent.transform.parent= transform.parent;
        itTransparent.tag ="ghost";
       

    }
    
    /// <summary>
    /// Updates the trail renderer, handles character death, and other functionalities.
    /// </summary>
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
                    theSmoke.transform.parent=transform;
                }else{
                    theSmoke = Instantiate(smokeDeath_red, transform.position, transform.rotation);
                    theSmoke.transform.parent=transform;
                }
                theSmoke.SetActive(true);
                
            }
            
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 1f * Time.deltaTime);
            
            if (Vector3.Distance(transform.localScale, Vector3.zero) < 0.2f){
                isDead = true;
                Destroy(gameObject);
                Destroy(theSmoke);
                createTransparent();

            }
        }
        
        
        
    }

   
}