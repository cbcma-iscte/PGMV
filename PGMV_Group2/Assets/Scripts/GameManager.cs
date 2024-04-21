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
            foreach (XmlNode unitNode in turnNode)
            {
                Unit unit;
                switch (unitNode.Attributes["type"].Value)
                {
                    case "archer":
                        unit = gameObject.AddComponent<Archer>();
                        break;
                    case "catapult":
                        unit = gameObject.AddComponent<Catapult>();
                        break;
                    case "mage":
                        unit = gameObject.AddComponent<Mage>();
                        break;
                    case "soldier":
                        unit = gameObject.AddComponent<Soldier>();
                        break;
                    default:
                        throw new System.Exception("Invalid unit type");
                }

                unit.Id = unitNode.Attributes["id"].Value;
                unit.Role = unitNode.Attributes["role"].Value;
                unit.Type = unitNode.Attributes["type"].Value;
                unit.Action = unitNode.Attributes["action"].Value;
                unit.X = int.Parse(unitNode.Attributes["x"].Value);
                unit.Y = int.Parse(unitNode.Attributes["y"].Value);

                turn.Units.Add(unit);
                Debug.Log($"Loaded unit: {unit.Id} ({unit.Type}) at ({unit.X}, {unit.Y})");
            }
            Debug.Log($"Loaded turn: {turnIndex + 1}");
            Debug.Log($"Turn has {turn.Units.Count} units");
            turnIndex++;
        }
    }
}
