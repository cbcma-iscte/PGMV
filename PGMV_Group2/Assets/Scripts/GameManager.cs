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
        Board = new Board();
        int Width = int.Parse(boardNode.Attributes["width"].Value);
        int Height = int.Parse(boardNode.Attributes["height"].Value);
        Board.InitializeBoard(Width, Height);
        
        Debug.Log($"Initializing board with dimensions {Board.Width}x{Board.Height}...");
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
