using UnityEngine;
using System.Xml;

public class XMLReader : MonoBehaviour
{
    public TextAsset xmlFile; // Reference to the XML file

    void Start()
    {
        ParseXML(xmlFile.text);
    }


    void ParseXML(string xmlText)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlText);

        // Get the root element
        XmlNode rootNode = xmlDoc.SelectSingleNode("game");

        // Get roles, board, and turns elements
        XmlNode rolesNode = rootNode.SelectSingleNode("roles");
        XmlNode boardNode = rootNode.SelectSingleNode("board");
        XmlNode turnsNode = rootNode.SelectSingleNode("turns");

        //Applies to Game Instance
        

        // Parse roles
        XmlNodeList roleNodes = rolesNode.SelectNodes("role");
        foreach (XmlNode roleNode in roleNodes)
        {
            string roleName = roleNode.Attributes["name"].Value;
            Debug.Log("Role: " + roleName);
        }

        // Parse board
        int width = int.Parse(boardNode.Attributes["width"].Value);
        int height = int.Parse(boardNode.Attributes["height"].Value);
        Debug.Log("Board Width: " + width);
        Debug.Log("Board Height: " + height);
        XmlNodeList squareNodes = boardNode.ChildNodes;
        foreach (XmlNode squareNode in squareNodes)
        {
            string groundType = squareNode.Name;
            Debug.Log("Ground Type: " + groundType);
        }

        // Parse turns
        XmlNodeList turnNodes = turnsNode.SelectNodes("turn");
        foreach (XmlNode turnNode in turnNodes)
        {
            XmlNodeList unitNodes = turnNode.SelectNodes("unit");
            foreach (XmlNode unitNode in unitNodes)
            {
                string id = unitNode.Attributes["id"].Value;
                string roleRefId = unitNode.Attributes["role"].Value;
                string type = unitNode.Attributes["type"].Value;
                string action = unitNode.Attributes["action"].Value;
                int x = int.Parse(unitNode.Attributes["x"].Value);
                int y = int.Parse(unitNode.Attributes["y"].Value);
                Debug.Log("Unit ID: " + id + ", RoleRefID: " + roleRefId + ", Type: " + type + ", Action: " + action + ", X: " + x + ", Y: " + y);
            }
        }
    }
}