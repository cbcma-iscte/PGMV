using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TerrainGenerator : MonoBehaviour
{

    static public int _TERRAIN_SCALE = 10;
    public Terrain terrain;
    public TextAsset xmlFile;
    public List<GameObject> treePrefabs;
    public List<GameObject> rockPrefabs;
    public List<GameObject> housePrefabs;

    public TerrainData terrainData;
    private Dictionary<string, SquareData> squareDataDict;

    // GameManager will change this value when changing scene and invoking this script
    public string tileType;

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

    void GenerateTerrain(SquareData squareData)
    {
        terrainData = terrain.terrainData;
        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        float maxElevation = squareData.MaximumElevation;

        // Set the maximum elevation
        terrainData.size = new Vector3(terrainData.size.x, maxElevation * _TERRAIN_SCALE, terrainData.size.z);
        Debug.Log("Generating terrain with maximum elevation: " + maxElevation);

        // Using multiple frequencies of Perlin Noise
        // Frequency determines hills and valleys. 
        // For not random terrain, you can set the frequency to a fixed value (e.g. 2 or 3 is a good start)
        // If you want to have more hills and valleys, increase the number of frequencies
        float frequency1 = Random.Range(2f, 4f);
        Debug.Log("Frequency 1: " + frequency1);
        float frequency2 = Random.Range(2f, 6f);
        Debug.Log("Frequency 2: " + frequency2);
        // Amplitude determines the weight of each frequency
        float amplitude1 = 0.5f;
        float amplitude2 = 0.5f;

        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                float xCoord = (float)x / terrainData.heightmapResolution;
                float yCoord = (float)y / terrainData.heightmapResolution;


                // Compute the noise value
                float elevation = (Mathf.PerlinNoise(xCoord * frequency1, yCoord * frequency1) * amplitude1 +
                                   Mathf.PerlinNoise(xCoord * frequency2, yCoord * frequency2) * amplitude2) /
                                  (amplitude1 + amplitude2);

                elevation = Mathf.Lerp(0, 1, Mathf.Clamp(elevation, 0.3f, 0.7f));

                // Scale to maximum elevation
                heights[x, y] = elevation;
            }
        }
        // Debugging WHY NOT SETTING HEIGHTS properly??? TODO
        terrain.terrainData.SetHeights(0, 0, heights);
        Debug.Log("Terrain generated");

    }

    public TerrainData GetTerraintData()
    {
        return terrainData;
    }

    void ScatterObjects(SquareData squareData)
    {
        int _EXPANSION_RANGE = 8;

        for (int x = _EXPANSION_RANGE; x < terrainData.heightmapResolution; x += _EXPANSION_RANGE)
        {
            for (int y = _EXPANSION_RANGE; y < terrainData.heightmapResolution; y += _EXPANSION_RANGE)
            {

                 // 513 x 513
                if ( x > (terrainData.heightmapResolution/2 - 50) && x < (terrainData.heightmapResolution/2 + 50) && y > (terrainData.heightmapResolution/2 - 50) && y < (terrainData.heightmapResolution/2 + 50))
                    continue;

                float height = terrainData.GetHeight(x, y) * _TERRAIN_SCALE / squareData.MaximumElevation;
                ObjectData objData = squareData.Objects[Random.Range(0, squareData.Objects.Count)];
                GameObject prefab = GetPrefabByType(objData.Type);
                float densityLow = objData.DensityLowAltitude;
                float densityHigh = objData.DensityHighAltitude;

                if (height < 0.2f && Random.value < densityLow)
                {
                    //Debug.Log("Placing object at: " + x + ", " + y);
                    PlaceObject(prefab, x, y);
                }
                else if (height > 0.8f && Random.value < densityHigh)
                {
                    PlaceObject(prefab, x, y);
                }
                // Percentage of height
                //Debug.Log("Height at " + x + ", " + y + ": " + height);
            }
        }
    }

    GameObject GetPrefabByType(string type)
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
