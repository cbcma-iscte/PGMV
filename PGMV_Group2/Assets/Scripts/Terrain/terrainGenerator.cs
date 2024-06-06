using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// The class is used to generate the terrain and scatter objects on the terrain
/// The terrain is generated based on the terrain data from the XML file
/// The objects are scattered based on the terrain elevation and the density of the objects in the XML file
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    /// <summary>
    /// The scale factor applied to terrain elevations. This value determines how much the generated terrain heights are multiplied by.
    /// </summary>
    static public int _TERRAIN_SCALE = 10;

    /// <summary>
    /// Terrain Object Unity
    /// </summary>
    public Terrain terrain;

    /// <summary>
    /// XML file containing the terrain data
    /// </summary>
    public TextAsset xmlFile;

    /// <summary>
    /// Prefabs for trees
    /// </summary>
    public List<GameObject> treePrefabs;

    /// <summary>
    /// Prefabs for rocks
    /// </summary>
    public List<GameObject> rockPrefabs;

    /// <summary>
    /// Prefabs for houses
    /// </summary>
    public List<GameObject> housePrefabs;

    /// <summary>
    /// Terrain data
    /// </summary>
    public TerrainData terrainData;

    /// <summary>
    /// Dictionary containing the square data from the XML file
    /// </summary>
    private Dictionary<string, SquareData> squareDataDict;

    /// <summary>
    /// Type of tile given by the GameManager
    /// </summary>
    public string tileType;

    /// <summary>
    /// Radius of the battle spot
    /// </summary>
    static float _BATTLE_SPOT_RADIUS = 30f;
    
    /// <summary>
    /// Radius of the interpolation area between the battle spot and the rest of the terrain
    /// </summary>
    static float _BATTLE_SPOT_RADIUS_INTERPOLATION = 75f;

    /// <summary>
    /// Start is called before the first frame update
    /// Parse the XML file and call the functions to generate the terrain and scatter objects
    /// based on the terrain Type given by the GameManager
    /// </summary>
    void Start()
    {
        TerrainParser parser = new TerrainParser { xmlFile = xmlFile };
        squareDataDict = parser.ParseXML();

        // Assuming you have a way to determine the current square type
        string currentSquareType = Staticdata.typeToCreateBattle;
        // Example: "forest", "desert", "mountain" for now only these 3 types are supported (Houses prefab missing)
        SquareData currentSquareData = squareDataDict[currentSquareType];

        GenerateTerrain(currentSquareData);
        ScatterObjects(currentSquareData);
    }

    /// <summary>
    /// The method is used to generate the terrain
    /// In the center of the terrain, the elevation is set to a constant value
    /// In the area of interpolation between the center and the rest of the terrain, the elevation is interpolated
    /// between the constant value and the Perlin Noise value
    /// In the rest of the terrain, the elevation is set to the Perlin Noise value using multiple frequencies
    /// </summary>
    /// <param name="squareData">The data of the square</param>
    void GenerateTerrain(SquareData squareData)
    {
        terrainData = terrain.terrainData;
        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        float maxElevation = squareData.MaximumElevation;

        // Set the maximum elevation
        terrainData.size = new Vector3(terrainData.size.x, maxElevation * _TERRAIN_SCALE, terrainData.size.z);

        // Using multiple frequencies of Perlin Noise
        // Frequency determines hills and valleys. 
        // For not random terrain, you can set the frequency to a fixed value (e.g. 2 or 3 is a good start)
        // If you want to have more hills and valleys, increase the number of frequencies
        float frequency1 = Random.Range(2f, 4f);
        float frequency2 = Random.Range(2f, 6f);
        // Amplitude determines the weight of each frequency
        float amplitude1 = 0.5f;
        float amplitude2 = 0.5f;

        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                float xCoord = (float)x / terrainData.heightmapResolution;
                float yCoord = (float)y / terrainData.heightmapResolution;

                // Calcola le coordinate del centro dell'area
                float centerX = terrainData.heightmapResolution / 2f; 
                float centerY = terrainData.heightmapResolution / 2f;

                // Determina la distanza dal centro
                float distanceFromCenter = Mathf.Sqrt(
                    Mathf.Pow(xCoord * terrainData.heightmapResolution - centerX, 2) +
                    Mathf.Pow(yCoord * terrainData.heightmapResolution - centerY, 2)
                );

                float elevation;
                // Calcola l'elevazione
                if (distanceFromCenter <= _BATTLE_SPOT_RADIUS) { // Area piana centrale 
                    elevation = 0.5f;
                } else if (distanceFromCenter <= _BATTLE_SPOT_RADIUS_INTERPOLATION) { // Area di interpolazione 
                    float interpolationFactor = (distanceFromCenter - _BATTLE_SPOT_RADIUS) / (_BATTLE_SPOT_RADIUS_INTERPOLATION-_BATTLE_SPOT_RADIUS); // Normalizza la distanza tra 0 e 1
                    elevation = Mathf.Lerp(
                        0.5f, // Valore costante al centro
                        (Mathf.PerlinNoise(xCoord * frequency1, yCoord * frequency1) * amplitude1 +
                        Mathf.PerlinNoise(xCoord * frequency2, yCoord * frequency2) * amplitude2) /
                        (amplitude1 + amplitude2),
                        interpolationFactor
                    );
                } else { // Area esterna (oltre 45 pixel di raggio)
                    elevation = (Mathf.PerlinNoise(xCoord * frequency1, yCoord * frequency1) * amplitude1 +
                                Mathf.PerlinNoise(xCoord * frequency2, yCoord * frequency2) * amplitude2) /
                                (amplitude1 + amplitude2);
                }

                elevation = Mathf.Lerp(0, 1, Mathf.Clamp(elevation, 0.3f, 0.7f)); 

                // Scale to maximum elevation
                heights[x, y] = elevation;
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);

    }

    /// <summary>
    /// The method is used to get the terrain data
    /// </summary>
    /// <returns>The terrain data</returns>
    private TerrainData GetTerraintData()
    {
        return terrainData;
    }

    /// <summary>
    /// The method is used to scatter objects on the terrain
    /// The objects are scattered based on the terrain elevation
    /// and the density of the objects in the XML file
    /// </summary>
    /// <param name="squareData">The data of the square</param>
    void ScatterObjects(SquareData squareData)
    {
        int _EXPANSION_RANGE = 8;

        for (int x = _EXPANSION_RANGE; x < terrainData.heightmapResolution; x += _EXPANSION_RANGE)
        {
            for (int y = _EXPANSION_RANGE; y < terrainData.heightmapResolution; y += _EXPANSION_RANGE)
            {
                float xCoord = (float)x / terrainData.heightmapResolution;
                float yCoord = (float)y / terrainData.heightmapResolution;

                // Calcola le coordinate del centro dell'area
                float centerX = terrainData.heightmapResolution / 2f; 
                float centerY = terrainData.heightmapResolution / 2f;

                // Determina la distanza dal centro
                float distanceFromCenter = Mathf.Sqrt(
                    Mathf.Pow(xCoord * terrainData.heightmapResolution - centerX, 2) +
                    Mathf.Pow(yCoord * terrainData.heightmapResolution - centerY, 2)
                );
                 // 513 x 513
                if ( distanceFromCenter <= _BATTLE_SPOT_RADIUS)
                    continue;

                float height = terrainData.GetHeight(x, y) / squareData.MaximumElevation;
                ObjectData objData = squareData.Objects[Random.Range(0, squareData.Objects.Count)];
                GameObject prefab = GetPrefabByType(objData.Type);
                float densityLow = objData.DensityLowAltitude;
                float densityHigh = objData.DensityHighAltitude;

                if (height < 0.2f && Random.value < densityLow)
                {
                    PlaceObject(prefab, x, y);
                }
                else if (height > 0.8f && Random.value < densityHigh)
                {
                    PlaceObject(prefab, x, y);
                }
            }
        }
    }

    /// <summary>
    /// The method is used to get the prefab based on the object type
    /// It selects a random prefab from the list of prefabs of a given type
    /// </summary>
    /// <param name="type">The type of the object</param>
    /// <returns>The prefab of the object</returns>
    private GameObject GetPrefabByType(string type)
    {
        switch (type)
        {
            case "tree":
                return treePrefabs[Random.Range(0, treePrefabs.Count)];
            case "rock":
                return rockPrefabs[Random.Range(0, rockPrefabs.Count)];
            case "house":
                return housePrefabs[Random.Range(0, housePrefabs.Count)];
            default:
                return null;
        }
    }

    /// <summary>
    /// The method is used to place an object on the terrain
    /// The object is placed at the given position
    /// it is placed at the height of the terrain an the rotation is randomized
    /// </summary>
    /// <param name="prefab">The prefab of the object to place</param>
    /// <param name="x">The x coordinate of the position</param>
    /// <param name="y">The y coordinate of the position</param>
    void PlaceObject(GameObject prefab, int x, int y)
    {
        float worldX = x / (float)terrainData.heightmapResolution * terrainData.size.x;
        float worldZ = y / (float)terrainData.heightmapResolution * terrainData.size.z;
        float worldY = terrainData.GetHeight(x, y);

        float prefabHeight = prefab.transform.position.y;
        worldY = worldY + prefabHeight;

        Vector3 position = new Vector3(worldX, worldY, worldZ);
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        obj.transform.Rotate(Vector3.up, Random.Range(0, 360));
        obj.transform.parent = terrain.transform;
    }
}
