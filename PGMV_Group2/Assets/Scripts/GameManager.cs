using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject Table;
    public List<Role> Roles { get; private set; }
    public Board Board { get; private set; }
    public List<GameObject> UnitHandler { get; private set; }
    [SerializeField] public Material[] tiles_materials;
    
    [SerializeField] private string xmlResourcePath;
    private XmlDocument xmlDoc = new XmlDocument();

    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private GameObject catapultPrefab;
    
    private void Awake()
    {
        
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

        Debug.Log("Game loaded successfully!");
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
            Roles.Add(new Role { Name = roleNode.Attributes["name"].Value });
        }
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
                    Debug.Log("Loaded tile: village");
                    break;
                case "forest":
                    tiles.Add(new Tile("forest"));
                    Debug.Log("Loaded tile: forest");
                    break;
                case "plain":
                    tiles.Add(new Tile("plain"));
                    Debug.Log("Loaded tile: plain");
                    break;
                case "desert":
                    tiles.Add(new Tile("desert"));
                    Debug.Log("Loaded tile: desert");
                    break;
                case "sea":
                    tiles.Add(new Tile("sea"));
                    Debug.Log("Loaded tile: sea");
                    break;
                case "mountain":
                    tiles.Add(new Tile("mountain"));
                    Debug.Log("Loaded tile: mountain");
                    break;
            }
        }
        Board = new Board();
        
        // Initialize the board with the provided width and height
        int board_width = int.Parse(boardNode.Attributes["width"].Value);
        int board_height = int.Parse(boardNode.Attributes["height"].Value);
        Board.InitializeBoard(board_width,board_height, tileAndMaterial, tiles,Table);

        Debug.Log($"Loading board with dimensions {Board.Width}x{Board.Height}...");

    }


    private void LoadTurns(XmlNode turnNodes)
    {
        UnitHandler = new List<GameObject>();

        foreach (XmlNode turnNode in turnNodes)
        {
            foreach (XmlNode unitNode in turnNode)
            {

                string id = unitNode.Attributes["id"]?.Value;
                string role = unitNode.Attributes["role"]?.Value;
                string type = unitNode.Attributes["type"]?.Value;
                string action = unitNode.Attributes["action"]?.Value;
                int x = int.Parse(unitNode.Attributes["x"]?.Value);
                int y = int.Parse(unitNode.Attributes["y"]?.Value);

                GameObject prefab = GetPrefabByType(type);
                if (prefab == null) continue;

                Unit unitComponent = prefab.GetComponent<Unit>();
                unitComponent.Initialize(id, role, type, action, x, y);
                UnitHandler.Add(prefab);
            
                ManageActions(action, x, y, unitComponent, prefab);

            }
        }
    }

    private void ManageActions(string action, int x, int y, Unit unit, GameObject prefab)
    {
        switch (action)
        {
            case "hold":
                unit.Hold();
                break;
            case "attack":
                unit.Attack();
                break;
            case "move_to":
                unit.MoveTo(Board,x, y);
                break;
            case "spawn":
                unit.Spawn(prefab,Board, x, y);
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
}
