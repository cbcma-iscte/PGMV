using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    public Terrain terrain;
    public TextAsset xmlFile;
    public List<GameObject> treePrefabs;
    public List<GameObject> rockPrefabs;
    public List<GameObject> housePrefabs;

    private TerrainData terrainData;
    private Dictionary<string, SquareData> squareDataDict;

    void Start()
    {
        TerrainParser parser = new TerrainParser { xmlFile = xmlFile };
        squareDataDict = parser.ParseXML();
        
        // Assuming you have a way to determine the current square type
        string currentSquareType = "forest"; // Example
        SquareData currentSquareData = squareDataDict[currentSquareType];

        GenerateTerrain(currentSquareData);
        ScatterObjects(currentSquareData);
    }

    void GenerateTerrain(SquareData squareData)
    {
        terrainData = terrain.terrainData;
        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        float maxElevation = squareData.MaximumElevation;
        terrainData.size = new Vector3(terrainData.size.x, maxElevation, terrainData.size.z);
        Debug.Log("Generating terrain with maximum elevation: " + maxElevation);

        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                float xCoord = (float)x / terrainData.heightmapResolution;
                float yCoord = (float)y / terrainData.heightmapResolution;

                // Using multiple frequencies of Perlin Noise
                float frequency1 = 3f;
                float frequency2 = 6f;
                float amplitude1 = 0.5f;
                float amplitude2 = 0.5f;

                // Compute the noise value
                float elevation = (Mathf.PerlinNoise(xCoord * frequency1, yCoord * frequency1) * amplitude1 +
                                   Mathf.PerlinNoise(xCoord * frequency2, yCoord * frequency2) * amplitude2) / 
                                  (amplitude1 + amplitude2);

                // Scale to maximum elevation
                float height = elevation * maxElevation;
                Debug.Log("Elevation at " + x + ", " + y + ": " + height);

                heights[x, y] = height;

            }
        }
        // Debugging
        terrain.terrainData.SetHeights(0, 0, heights);
        Debug.Log("Terrain generated");
    }


    void ScatterObjects(SquareData squareData)
    {
        foreach (ObjectData objData in squareData.Objects)
        {
            GameObject prefab = GetPrefabByType(objData.Type);
            float densityLow = objData.DensityLowAltitude;
            float densityHigh = objData.DensityHighAltitude;
            Debug.Log("Scattering objects of type: " + objData.Type);
            Debug.Log("Density low: " + densityLow);
            Debug.Log("Density high: " + densityHigh);

            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                for (int y = 0; y < terrainData.heightmapResolution; y++)
                {
                    float height = terrainData.GetHeight(x, y) / squareData.MaximumElevation;
                    Debug.Log("Height at " + x + ", " + y + ": " + height);
                    if (height < 0.2f && Random.value < densityLow)
                    {
                        Debug.Log("Placing object at: " + x + ", " + y);
                        PlaceObject(prefab, x, y);
                    }
                    else if (height > 0.8f && Random.value < densityHigh)
                    {
                        //PlaceObject(prefab, x, y);
                    }
                }
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

        Vector3 position = new Vector3(worldX, worldY, worldZ);
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        obj.transform.Rotate(Vector3.up, Random.Range(0, 360));
        obj.transform.parent = terrain.transform;
    }
}
