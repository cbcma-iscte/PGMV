using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class GameManager : MonoBehaviour
{
    public List<Role> Roles { get; private set; }
    public Board Board { get; private set; }
    public List<Turn> Turns { get; private set; }

    [SerializeField] public string xmlResourcePath;

    public XmlDocument xmlDoc = new();

    [SerializeField] GameObject archerPrefab;
    [SerializeField] GameObject magePrefab;
    [SerializeField] GameObject soldierPrefab;
    [SerializeField] GameObject catapultPrefab;

    private void Awake(){
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
        Board = new Board();
        
         
        // Initialize the board with the provided width and height
        Board.InitializeBoard(int.Parse(boardNode.Attributes["width"].Value), int.Parse(boardNode.Attributes["height"].Value));

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
            foreach (XmlNode unitNode in turnNode)
            {

                string Id = unitNode.Attributes["id"].Value;
                string Role = unitNode.Attributes["role"].Value;
                string Type = unitNode.Attributes["type"].Value;
                string Action = unitNode.Attributes["action"].Value;
                int X = int.Parse(unitNode.Attributes["x"].Value);
                int Y = int.Parse(unitNode.Attributes["y"].Value);

                if (Action == "spawn")//This only has archer
                //1st spawn and add tag, 2nd had to turn, 3rd do action according to unit in turn
                {
                    Unit unit = new Unit(Id, Role, Type, Action, X, Y);
                    turn.Units.Add(unit);
                    GameObject unitPrefab = unit.spawn(archerPrefab);
                    unitPrefab.tag = "Archer";
                }
                

            }
            Debug.Log($"Loaded turn: {turnIndex + 1}");
            Debug.Log($"Turn has {turn.Units.Count} units");
            turnIndex++;
        }
    }

}
