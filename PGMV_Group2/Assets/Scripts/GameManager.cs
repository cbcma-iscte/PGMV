using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// The GameManager class handles the main game logic, including loading game data, managing turns, and controlling game state.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject Table;
    public List<Role> Roles { get; private set; }
    
    public List<Turn> Turns {get; set;} = new List<Turn>();
    public Board Board { get; private set; }
    [SerializeField] public Material[] tiles_materials;
    
    [SerializeField] private string xmlResourcePath;
    private XmlDocument xmlDoc = new XmlDocument();

    [SerializeField] public GameObject Player1;
    [SerializeField] public GameObject Player2;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private GameObject catapultPrefab;

    [SerializeField] 
    public int currentTurn = 0;
    public bool isLoadingScenes = true;
    private bool isRestarting = false;
    private int pointsPlayer1 = 0;
    private int pointsPlayer2 = 0;
    private bool isKillingGhosts = false;
    public bool isAutomatic = false;
    public bool isPaused = false;
    public bool isPlaying = false;
    public List<int> allPointsP1 = new List<int>();
    public List<int> allPointsP2 = new List<int>();

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Table);
        isKillingGhosts = false;
        Player1.SetActive(false);
        Player2.SetActive(false);
        
        InitializeGame();
    }

    ///<summary>
    ///This method initializes the game by loading game data from XML files, including roles, board layout, and turns.
    ///</summary>
    private void InitializeGame()
    {
         // Load XML data
        TextToXml(Resources.Load<TextAsset>(xmlResourcePath));

        // Load game elements from the XmlDocument
        var gameNode = xmlDoc.DocumentElement;
        var rolesNode = gameNode["roles"];
        var boardNode = gameNode["board"];
        var turnsNode = gameNode["turns"];

        //Load Roles
        LoadRoles(rolesNode);

        // Load Board
        LoadBoard(boardNode);

        // Load turns
        LoadTurns(turnsNode);

    }

    /// <summary>
    /// Loads XML data from a given TextAsset and populates the xmlDoc field.
    /// </summary>
    /// <param name="xmlTextAsset">The XML file to load</param>
    private void TextToXml(TextAsset xmlTextAsset)
    {
        if (xmlTextAsset == null)
        {
            Debug.LogError("XML file not found at path: " + xmlResourcePath);
            return;
        }
        xmlDoc.LoadXml(xmlTextAsset.text);
    }

    /// <summary>
    /// Loads role data from an XmlNode and populates the Roles list.
    /// </summary>
    /// <param name="rolesNode">The XmlNode containing role data</param>
    private void LoadRoles(XmlNode rolesNode)
    {
        Roles = new List<Role>();
        
        foreach (XmlNode roleNode in rolesNode)
        {
            Roles.Add(new Role { Name = roleNode.Attributes["name"].Value});
            
        }
        changeInforPontuation();
    }
    

    /// <summary>
    /// Loads board data from an XmlNode and initializes the game board with the specified tiles and dimensions.
    /// </summary>
    /// <param name="boardNode">The XmlNode containing board data</param>
    private void LoadBoard(XmlNode boardNode){
        List<Tile> tiles = new();
        
        Dictionary<string,Material> tileAndMaterial = new Dictionary<string,Material>{
            { "village", tiles_materials[4] },
            { "forest", tiles_materials[0] },
            { "plain", tiles_materials[3] },
            { "sea", tiles_materials[5] },
            { "desert", tiles_materials[2] },
            { "mountain", tiles_materials[1] }

        };
        foreach (XmlNode tileNode in boardNode)
        {
            switch (tileNode.Name)
            {
                case "village":
                    tiles.Add(new Tile("village"));
                    break;
                case "forest":
                    tiles.Add(new Tile("forest"));
                    break;
                case "plain":
                    tiles.Add(new Tile("plain"));
                    break;
                case "desert":
                    tiles.Add(new Tile("desert"));
                    break;
                case "sea":
                    tiles.Add(new Tile("sea"));
                    break;
                case "mountain":
                    tiles.Add(new Tile("mountain"));
                    break;
            }
        }
        Board = new Board();
        
        int board_width = int.Parse(boardNode.Attributes["width"].Value);
        int board_height = int.Parse(boardNode.Attributes["height"].Value);
        Board.InitializeBoard(board_width,board_height, tileAndMaterial, tiles,Table,Roles);

    }

    /// <summary>
    /// Loads turn data from an XmlNode and populates the Turns list with Turn objects containing information about units' actions.
    /// </summary>
    /// <param name="turnNodes">The XmlNode containing turn data</param>
    private void LoadTurns(XmlNode turnNodes)
    {
        int turnId = 1;
        foreach (XmlNode turnNode in turnNodes)
        {
            Turn newTurn = new(turnId);
            foreach (XmlNode unitNode in turnNode)
            {

                string id = unitNode.Attributes["id"]?.Value;
                string role = unitNode.Attributes["role"]?.Value;
                string type = unitNode.Attributes["type"]?.Value;
                string action = unitNode.Attributes["action"]?.Value;
                int x = int.Parse(unitNode.Attributes["x"]?.Value);
                int y = int.Parse(unitNode.Attributes["y"]?.Value);


                Unit unitComponent = new Unit(id, role, type, x, y, action);
                newTurn.Units.Add(unitComponent);
            }
            turnId++;
            Turns.Add(newTurn);
        }

        PlayGame();
    }

    /// <summary>
    /// Pauses or resumes the game by adjusting the time scale.
    /// </summary>
    public void PauseResumeGame(){
        if(isPaused){
            Time.timeScale = 1f;
        }else{
            Time.timeScale = 0f;
        }
        
        isPaused = !isPaused;
    }
    /// <summary>
    /// Executes the game logic by processing each turn, including unit actions such as spawning, moving, holding, and attacking.
    /// </summary>
    /// <returns>An IEnumerator coroutine</returns>
    public IEnumerator PlayGame(){
        isPlaying=true;
        foreach(Turn turn in Turns)
        {
            if(turn.Id == currentTurn)
            {
                foreach(Unit unit in turn.Units)
                {
                    if(unit.Action == "spawn" || unit.Action == "move_to" || unit.Action == "hold"){
                        ManageActions(unit);
                        if(Board.findCharacterInBoard(unit).GetComponent<Character>().canMove){
                            yield return StartCoroutine(WaitMovement(Board.findCharacterInBoard(unit).GetComponent<Character>()));
                        }
                    }
                   
                }
                foreach(Unit unit in turn.Units)
                {
                    if(unit.Action == "attack"){
                        ManageActions(unit);
                    }
                    
                }
            }
            
        }
        if(isAutomatic){
            yield return StartCoroutine(waitForTurn());
        }

        verifyBattles();
        allPointsP1.Add(pointsPlayer1);
        allPointsP2.Add(pointsPlayer2);
        isPlaying=false;
        

    }

    /// <summary>
    /// Checks for battles in the current turn and loads the battle scene if the isLoadingScenes is true.
    /// </summary>
    public void verifyBattles(){
       if(Board.battlesInTurn.Count>0){
            if (isLoadingScenes){
                Debug.Log("Loading scenes" + isLoadingScenes);
                foreach (string typeOfBattle in Board.battlesInTurn){
                    Staticdata.typeToCreateBattle = typeOfBattle.ToLower().Split(' ')[0];
                    GameObject[] menu = GameObject.FindGameObjectsWithTag("MenuInformation");
                    if (menu.Length > 0)
                    {
                        menu[0].SetActive(false);
                    }
                    SceneManager.LoadScene("TerrainScene");
                }
            }
            Board.battlesDelivered();
       }

    }
    /// <summary>
    /// Waits for a character to finish its movement before proceeding.
    /// </summary>
    /// <param name="character">The character to wait for</param>
    /// <returns>An IEnumerator coroutine</returns>
    public IEnumerator WaitMovement(Character character){
        while(character.canMove){
            yield return null; 
        }
       
    }

    /// <summary>
    /// Waits for a turn to finish before proceeding.
    /// </summary>
    /// <returns>An IEnumerator coroutine</returns>
    private IEnumerator waitForTurn(){
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// Advances the game to the next turn.
    /// </summary>
    public void GoForward()
    {
        if(isPlaying==false && isRestarting==false){
        if(currentTurn + 1<Turns.Count){
            currentTurn++;
            StartCoroutine(PlayGame());
        }else{
            
        }
        }
    }

    /// <summary>
    /// Moves the game back to the previous turn.
    /// </summary>
    public void GoBack()
    {
        if(currentTurn>0){
            if(isPlaying==false && isRestarting==false){
                currentTurn--;
                Redo(currentTurn+1);
            }
        }
    }

    /// <summary>
    /// Reverts the actions performed in a specific turn and updates the game state accordingly.
    /// </summary>
    /// <param name="id">The ID of the turn</param>
    public void Redo(int id){
        foreach(Turn turn in Turns){
            if(turn.Id == id-1){
                
            }
            if(turn.Id == id){
                goBackOnce(turn);
            }
        }
    }

    
    /// <summary>
    /// Reverts the actions performed in a single turn.
    /// </summary>
    /// <param name="turn">The Turn object representing the turn to redo</param>
    private void goBackOnce(Turn turn){
        foreach (Unit unit in turn.Units){
            if(Board.findCharacterInBoard(unit) == null){
                unit.Action = "spawn"; 
                ManageActions(unit);
            }else{
                if(unit.Action == "spawn"){
                    foreach(Transform child in Board.getBoardByName()){
                        if(child.GetComponent<Character>() != null && child.tag!="Tile"){
                        if(child.GetComponent<Character>().Id==unit.Id){
                            Destroy(child.gameObject);
                        }
                        }
                    }
                }else if(unit.Action == "attack"){
                        allPointsP1.RemoveAt(allPointsP1.Count - 1);
                        allPointsP2.RemoveAt(allPointsP2.Count - 1);
                        Board.changePontuation(allPointsP1[allPointsP1.Count-1],allPointsP2[allPointsP2.Count-1]);    
                        changeInforPontuation();
                }else if(unit.Action == "move_to"){
                    Unit previousPosition = find_previousUnit(turn.Id,unit);
                    Board.findCharacterInBoard(unit).GetComponent<Character>().MoveTo(Board,previousPosition.X,previousPosition.Y);
                }else{
                    if(verifyMovement(turn.Id,unit)){
                        Unit previousPosition = find_previousUnit(turn.Id,unit);
                        Board.findCharacterInBoard(unit).GetComponent<Character>().MoveTo(Board,previousPosition.X,previousPosition.Y);
                
                    }else{
                    ManageActions(unit);  
                    }
                }
                
            }
            
        }
    }

    /// <summary>
    /// Verifies if a unit's previous action was movement.
    /// </summary>
    /// <param name="id">The ID of the turn containing the unit's previous action</param>
    /// <param name="unit">The unit to verify</param>
    /// <returns>True if the previous action was movement, otherwise false</returns>
    private bool verifyMovement(int id, Unit unit ){
        
        foreach(Turn turn in Turns){
            if(turn.Id == id){
                Unit unitNew = findUnit(turn, unit);
                if(unitNew.Action == "move_to") return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// Finds the unit's previous position in a turn.
    /// </summary>
    /// <param name="id">The ID of the turn containing the unit's previous position</param>
    /// <param name="unit">The unit to find</param>
    /// <returns>The unit's previous position</returns>
    private Unit find_previousUnit(int id,Unit unit){
        foreach(Turn turn in Turns){
            if(turn.Id + 1 == id){
                return findUnit(turn, unit);
            }
        }
        return null;
    }

    /// <summary>
    /// Finds a specific unit in a turn.
    /// </summary>
    /// <param name="turn">The Turn object to search</param>
    /// <param name="unit">The unit to find</param>
    /// <returns>The found unit</returns>
    private Unit findUnit(Turn turn, Unit unit){
        foreach(Unit units in turn.Units){
            if(units.Id==unit.Id){
                return units;
            }
        }
        return null;
    }

    /// <summary>
    /// Restarts the game by resetting the current turn, player points, and game state.
    /// </summary>
    public void RestartGame()
    {   
        isRestarting = true;
        currentTurn = 0;
        pointsPlayer1 = 0;
        pointsPlayer2 = 0;
        isPlaying = false;

        Board.restartPontuation();
        changeInforPontuation();
        foreach(Transform child in Board.getBoardByName())
        {
                if(child.tag!="Tile")
                    Destroy(child.gameObject);
        }
        isRestarting = false;
       

    }
    
    
    /// <summary>
    /// Manages the actions of a unit based on its action type.
    /// </summary>
    /// <param name="unit">The unit whose action to manage</param>
    private void ManageActions(Unit unit)
    {      
        switch (unit.Action)
        {
            case "hold":
                Board.findCharacterInBoard(unit).GetComponent<Character>().Hold();
                break;
            case "attack":
                Board.findCharacterInBoard(unit).GetComponent<Character>().Attack(Board,unit.X,unit.Y);
                break;
            case "move_to":
                Board.findCharacterInBoard(unit).GetComponent<Character>().MoveTo(Board,unit.X,unit.Y);
                break;
            case "spawn":
                GameObject prefab = GetPrefabByType(unit.Type);
                prefab.GetComponent<Character>().Spawn(prefab,Board, unit.X, unit.Y,unit.Role,unit.Id,Roles);
                break;
        }
        
    }

    /// <summary>
    /// Retrieves the prefab GameObject associated with a specific unit type.
    /// </summary>
    /// <param name="type">The type of unit</param>
    /// <returns>The prefab GameObject for the specified unit type</returns>
    private GameObject GetPrefabByType(string type)
    {
        switch (type.ToLower())
        {
            case "archer": 
                return archerPrefab;
            case "mage": 
                return magePrefab;
            case "soldier": 
                return soldierPrefab;
            case "catapult": 
                return catapultPrefab;
            default:
                Debug.LogError($"No prefab found for type: {type}");
                return null;
        }
    }

    /// <summary>
    /// Destroys a GameObject after a specified delay.
    /// </summary>
    /// <param name="child">The GameObject to destroy</param>
    /// <returns>An IEnumerator for yielding during the delay</returns>
    private IEnumerator timeTodestroy(GameObject child){
        yield return new WaitForSeconds(5f);
        Destroy(child);
        isKillingGhosts=false;
    }
    
    /// <summary>
    /// Shows or hides the player point UI elements based on the specified parameter.
    /// </summary>
    /// <param name="isToShow">True to show the UI elements, false to hide them</param>
    public void showPontuations(bool isToShow){
        Player1.SetActive(isToShow);
        Player2 .SetActive(isToShow);
    }

    /// <summary>
    /// Updates the player information displayed in the UI with the current points for each player.
    /// </summary>
    private void changeInforPontuation(){
        Player1.GetComponentInChildren<TextMeshProUGUI>().text = Roles[0].Name + ": " + pointsPlayer1;
        Player2.GetComponentInChildren<TextMeshProUGUI>().text = Roles[1].Name + ": " + pointsPlayer2;
    }

    /// <summary>
    /// Adds or reduces points for a specific player and updates the player's information in the UI.
    /// </summary>
    /// <param name="nameOfPlayer">The name of the player to update</param>
    /// <param name="point">The number of points to add or reduce</param>
    public void addReducePoints(string nameOfPlayer, int point){
        if(nameOfPlayer == Roles[0].Name){
            pointsPlayer1 = pointsPlayer1 + point;
            Player1.GetComponentInChildren<TextMeshProUGUI>().text = Roles[0].Name + ": " + pointsPlayer1;
            
        }else{
            pointsPlayer2 = pointsPlayer2 + point;
            Player2.GetComponentInChildren<TextMeshProUGUI>().text = Roles[1].Name + ": " + pointsPlayer2;
            
        }
    }
        
    /// <summary>
    /// Updates the game state based on certain conditions.
    /// It checks if there are any ghost GameObjects in the scene.
    /// It updates the player points and UI information if the game is not currently playing.
    /// </summary>
    void Update(){
        
        if(isKillingGhosts == false){
            GameObject[] Ghosts = GameObject.FindGameObjectsWithTag("ghost");
            if(Ghosts.Length>0){
                isKillingGhosts =true;
                foreach(GameObject ghost in Ghosts){
                    StartCoroutine(timeTodestroy(ghost));
                }
            }
        }

        if(isPlaying==false){
            pointsPlayer1 = Board.pontuationPlayer1;
            pointsPlayer2 = Board.pontuationPlayer2;
            changeInforPontuation();

        }
        
    }

    

    
}
