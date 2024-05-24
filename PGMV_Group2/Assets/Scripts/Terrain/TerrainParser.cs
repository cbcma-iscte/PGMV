using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class TerrainParser : MonoBehaviour
{
    public TextAsset xmlFile;

    public Dictionary<string, SquareData> ParseXML()
    {
        Dictionary<string, SquareData> squareDataDict = new Dictionary<string, SquareData>();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFile.text);

        XmlNodeList squares = xmlDoc.GetElementsByTagName("square");

        foreach (XmlNode square in squares)
        {
            SquareData squareData = new SquareData();
            squareData.Type = square.Attributes["type"].Value;
            squareData.MaximumElevation = float.Parse(square.Attributes["maximum_elevation"].Value);

            XmlNodeList objects = square.SelectNodes("object");

            foreach (XmlNode obj in objects)
            {
                ObjectData objectData = new ObjectData();
                objectData.Type = obj.Attributes["type"].Value;
                objectData.DensityLowAltitude = float.Parse(obj.Attributes["density_low_altitute"].Value, CultureInfo.InvariantCulture);
                objectData.DensityHighAltitude = float.Parse(obj.Attributes["density_high_altitute"].Value, CultureInfo.InvariantCulture);
                squareData.Objects.Add(objectData);
                Debug.Log("Parsed object: " + objectData.Type);
                Debug.Log("Density low: " + objectData.DensityLowAltitude);
                Debug.Log("Density high: " + objectData.DensityHighAltitude);
            }
            
            squareDataDict.Add(squareData.Type, squareData);
            Debug.Log("Parsed square: " + squareData.Type);
        }

        return squareDataDict;
    }
}

public class SquareData
{
    public string Type;
    public float MaximumElevation;
    public List<ObjectData> Objects = new List<ObjectData>();
}

public class ObjectData
{
    public string Type;
    public float DensityLowAltitude;
    public float DensityHighAltitude;
}
