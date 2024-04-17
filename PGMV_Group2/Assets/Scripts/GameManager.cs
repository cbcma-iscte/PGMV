using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class GameManager : MonoBehaviour
{
    public List<Role> Roles { get; set; }
    public Board Board { get; set; }
    public List<Turn> Turns { get; set; }

    [SerializeField]
    public string xmlResourcePath;

    public XmlDocument xmlDoc = new();

    void Awake()
    {
        xmlDoc.LoadXml(Resources.Load<TextAsset>(xmlResourcePath).text);

        // Load game elements from the XmlDocument
        var gameNode = xmlDoc.DocumentElement;
        var rolesNode = gameNode["roles"];
        var boardNode = gameNode["board"];
        var turnsNode = gameNode["turns"];

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

        // Load board
        Board = new Board
        {
            Tiles = new List<Tile>(),
            Width = int.Parse(boardNode.Attributes["width"].Value),
            Height = int.Parse(boardNode.Attributes["height"].Value)
        };
        Debug.Log($"Loading board with dimensions {Board.Width}x{Board.Height}...");
        foreach (XmlNode tileNode in boardNode)
        {
            switch (tileNode.Name)
            {
                case "village":
                    Board.Tiles.Add(new Tile("village"));
                    Debug.Log("Loaded tile: village");
                    break;
                case "forest":
                    Board.Tiles.Add(new Tile("forest"));
                    Debug.Log("Loaded tile: forest");
                    break;
                case "plain":
                    Board.Tiles.Add(new Tile("plain"));
                    Debug.Log("Loaded tile: plain");
                    break;
                case "desert":
                    Board.Tiles.Add(new Tile("desert"));
                    Debug.Log("Loaded tile: desert");
                    break;
                case "sea":
                    Board.Tiles.Add(new Tile("sea"));
                    Debug.Log("Loaded tile: sea");
                    break;
                case "mountain":
                    Board.Tiles.Add(new Tile("mountain"));
                    Debug.Log("Loaded tile: mountain");
                    break;
            }
        }

        // Load turns
        Turns = new List<Turn>();
        int turnIndex = 0;
        Debug.Log("Loading turns...");
        foreach (XmlNode turnNode in turnsNode)
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
        Debug.Log("Game loaded successfully!");
    }
}