using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class GameManager : MonoBehaviour
{
<<<<<<< HEAD

    public GameObject archer;
   
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(archer, new Vector3(-(2+i), (float)1.87600005, -1), Quaternion.identity);
        }
        
=======
    public List<Role> Roles { get; private set; }
    public Board Board { get; private set; }
    public List<Turn> Turns { get; private set; }

    [SerializeField] public string xmlResourcePath;

    public XmlDocument xmlDoc = new();

    private void Awake(){
        InitializeGame();
>>>>>>> main
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

    private void TextToXml(TextAsset xmlTextAsset){
         if (xmlTextAsset == null)
        {
            Debug.LogError("XML file not found at path: " + xmlResourcePath);
            return;
        }

        xmlDoc.LoadXml(xmlTextAsset.text);
    }

    private void LoadRoles(XmlNode rolesNode){
        // Load roles
        Roles = new List<Role>();
        Debug.Log("Loading roles...");
        foreach (XmlNode roleNode in rolesNode)
        {
            var role = new Role
            {
                Name = roleNode.Attributes["name"].Value
            };
            Roles.Add(role);
            Debug.Log($"Loaded role: {role.Name}");
        }
    }

    private void LoadBoard(XmlNode boardNode){
        List<Tile> tiles = new();
        
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
         Board = new Board
        {
            Tiles = tiles,
            Width = int.Parse(boardNode.Attributes["width"].Value),
            Height = int.Parse(boardNode.Attributes["height"].Value)
        };

         Debug.Log($"Loading board with dimensions {Board.Width}x{Board.Height}...");

    }

    private void LoadTurns(XmlNode turnNodes){
        Turns = new List<Turn>();
        int turnIndex = 0;
        Debug.Log("Loading turns...");
        foreach (XmlNode turnNode in turnNodes)
        {
            var turn = new Turn
            {
                Units = new List<Unit>()
            };
            Turns.Add(turn);
            Debug.Log($"Loaded turn: {turnIndex + 1}");
            int unitIndex = 0;
            foreach (XmlNode unitNode in turnNode)
            {
                var unit = new Unit
                {
                    Id = unitNode.Attributes["id"].Value,
                    Role = unitNode.Attributes["role"].Value,
                    Type = unitNode.Attributes["type"].Value,
                    Action = unitNode.Attributes["action"].Value,
                    X = int.Parse(unitNode.Attributes["x"].Value),
                    Y = int.Parse(unitNode.Attributes["y"].Value)
                };
                turn.Units.Add(unit);
                Debug.Log($"Loaded unit: {unit.Id} ({unit.Type}) at ({unit.X}, {unit.Y})");
                unitIndex++;
            }
            turnIndex++;
        }
    }
}
