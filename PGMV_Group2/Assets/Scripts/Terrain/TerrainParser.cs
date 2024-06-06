using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

/// <summary>
/// Parses the XML file containing the terrain data.
/// </summary>
public class TerrainParser : MonoBehaviour
{
    /// <summary>
    /// The XML file containing the terrain data.
    /// </summary>
    public TextAsset xmlFile;

    /// <summary>
    /// Parses the XML file and returns a dictionary containing the terrain data.
    /// The key is the square type and the value is the SquareData object containing the maximum elevation and the objects data.
    /// </summary>
    /// <returns>The dictionary containing the terrain data.</returns>
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
            }
            
            squareDataDict.Add(squareData.Type, squareData);
        }

        return squareDataDict;
    }
}

/// <summary>
/// Contains the data of a square.
/// </summary>
public class SquareData
{
    public string Type;
    public float MaximumElevation;
    public List<ObjectData> Objects = new List<ObjectData>();
}

/// <summary>
/// Contains the data of an object in a square.
/// </summary>
public class ObjectData
{
    public string Type;
    public float DensityLowAltitude;
    public float DensityHighAltitude;
}
