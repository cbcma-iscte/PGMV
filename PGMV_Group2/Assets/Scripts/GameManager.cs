using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;
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
    private bool isRestarting = false;
    private int pointsPlayer1 = 0;
    private int pointsPlayer2 = 0;
    private bool isKillingGhosts = false;
    public bool isAutomatic = false;
    public bool isPaused = false;
    public bool isPlaying = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Table);
        isKillingGhosts = false;
        Player1.SetActive(false);
        Player2.SetActive(false);
        
        InitializeGame();
    }

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

    private void TextToXml(TextAsset xmlTextAsset)
    {
        if (xmlTextAsset == null)
        {
            Debug.LogError("XML file not found at path: " + xmlResourcePath);
            return;
        }
        xmlDoc.LoadXml(xmlTextAsset.text);
    }

    private void LoadRoles(XmlNode rolesNode)
    {
        Roles = new List<Role>();
        
        foreach (XmlNode roleNode in rolesNode)
        {
            Roles.Add(new Role { Name = roleNode.Attributes["name"].Value});
            
        }
        changeInforPontuation();
    }

    private void changeInforPontuation(){
        Player1.GetComponentInChildren<TextMeshProUGUI>().text = Roles[0].Name + ": " + pointsPlayer1;
        Player2.GetComponentInChildren<TextMeshProUGUI>().text = Roles[1].Name + ": " + pointsPlayer2;
    }

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

        //Debug.Log($"Loading board with dimensions {Board.Width}x{Board.Height}...");

    }


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

    public void PauseResumeGame(){
        if(isPaused){
            Time.timeScale = 1f;
        }else{
            Time.timeScale = 0f;
        }
        
        isPaused = !isPaused;
    }
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
        isPlaying=false;
        

    }

    public void verifyBattles(){
       if(Board.battlesInTurn.Count>0){
            foreach (string typeOfBattle in Board.battlesInTurn){
                Staticdata.typeToCreateBattle = typeOfBattle.ToLower().Split(' ')[0];
                SceneManager.LoadScene("TerrainScene");
            
            }
            Board.battlesDelivered();
       }

    }
    public IEnumerator WaitMovement(Character character){
        while(character.canMove){
            yield return null; 
        }
       
    }
    private IEnumerator waitForTurn(){
        yield return new WaitForSeconds(1f);
    }

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


    public void GoBack()
    {
        if(isPlaying==false && isRestarting==false){
        currentTurn--;
        if(currentTurn>0){
            StartCoroutine(PlayGame());
        }
        }
    }

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

    private IEnumerator timeTodestroy(GameObject child){
        yield return new WaitForSeconds(5f);
        Destroy(child);
        isKillingGhosts=false;
    }
    
    public void showPontuations(bool isToShow){
        Player1.SetActive(isToShow);
        Player2 .SetActive(isToShow);
    }

    public void addReducePoints(string nameOfPlayer, int point){
        if(nameOfPlayer == Roles[0].Name){
            pointsPlayer1 = pointsPlayer1 + point;
            Player1.GetComponentInChildren<TextMeshProUGUI>().text = Roles[0].Name + ": " + pointsPlayer1;
            
        }else{
            pointsPlayer2 = pointsPlayer2 + point;
            Player2.GetComponentInChildren<TextMeshProUGUI>().text = Roles[1].Name + ": " + pointsPlayer2;
            
        }
    }
        
    
    void Update(){
        if(isKillingGhosts == false){
            GameObject[] Ghosts = GameObject.FindGameObjectsWithTag("ghost");
            if(Ghosts.Length>0){
                //Debug.Log("I'll start killing ghosts!");
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
