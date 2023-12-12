using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class WorldGeneration : MonoBehaviour
{
    // Tilemaps for biomes and resources
    public Tilemap tilemapBiomes;
    public Tilemap tilemapResources;

    // Controllers for tilemaps
    public TilemapBiomesController tilemapBiomesController;
    public TilemapResourcesController tilemapResourcesController;

    // List of controllers for tilemaps
    private List<BaseTilemapController> tilemapControllers = new List<BaseTilemapController>();

    // Swamp object to generate swamp borders
    [SerializeField]
    public Swamp swamp;
    // Generation parameters
    private float landformScale = 1;
    private float resourceRate = 1;//0.66 - 1.5

    // Seed for random generation
    private string seedString = "";
    public int seed = 0;

    //World parameters
    private int mapWidth = 100;
    private int islandRadius = 40;
    private Vector2Int islandCenter;
    private float noiseScale = 19f;

    //Lists for storing generated data
    public List<float> biomeList = new List<float>();
    public FloatListWrapper biomeWrappedList;

    Random random = new Random();

    //Managers for tiles and UI
    public TilesManager tileManager;
    public UIManager uiManager;

    public int MapWidth { get => mapWidth; set => mapWidth = value; }
    public string SeedString { get => seedString; set => seedString = value; }
    public int IslandRadius { get => islandRadius; set => islandRadius = value; }
    public float LandformScale { get => landformScale; set => landformScale = value; }
    public float ResourceRate { get => resourceRate; set => resourceRate = value; }
    private void Start()
    {    // Add tilemap controllers to the list
        try
        {
            if (tilemapBiomesController == null)
            {
                throw new NullReferenceException("TilemapBiomesController not found.");
            }

            if (tilemapResourcesController == null)
            {
                throw new NullReferenceException("TilemapResourcesController not found.");
            }

            tilemapControllers.Add(tilemapBiomesController);
            tilemapControllers.Add(tilemapResourcesController);

            GenerateWorld();
        }
        catch (Exception e)
        {
            Debug.LogError("Error in Start: " + e.Message);
        }
    }
    // Clear all tiles and data in the world
    public void ClearWorld()
    {
        tilemapControllers.ForEach(controller => controller.ClearAllTiles());
        biomeList.Clear();
        tilemapBiomesController.tilemapChars = new char[MapWidth, MapWidth];
        GenerateEmptyWorld();
    }

    // Generate an empty world with water biomes
    public void GenerateEmptyWorld()
    {
        EarlyGWInitialization();
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapWidth; y++)
            {
                tilemapBiomes.SetTile(new Vector3Int(x, y, 0), tileManager.tileList[0].tile);
                tilemapBiomesController.tilemapChars[x, y] = BiomeType.WATER;
                biomeList.Add(0f);
            }
        }
        LateGWInitialization();
    }

    // Generate the world with various biomes and resources
    public void GenerateWorld()
    {
        EarlyGWInitialization();

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapWidth; y++)
            {
                Vector3Int cellPosition;
                float swampNoise, treeNoise, biome;

                // Calculate noises and cell position
                CalculateNoises(x, y, out cellPosition, out swampNoise, out treeNoise, out biome);

                if (biome > 0.1f)
                {
                    biome = biome * LandformScale;
                }

                biomeList.Add(biome);
                // Use controllers to generate tiles and resources
                foreach (BaseTilemapController controller in tilemapControllers)
                {
                    controller.SetGeneratedTile(biome, swampNoise, treeNoise, cellPosition);
                }
            }
        }
        LateGWInitialization();
    }

    // Calculate noises and cell position based on coordinates
    private void CalculateNoises(int x, int y, out Vector3Int cellPosition, out float swampNoise, out float treeNoise, out float biome)
    {
        cellPosition = new Vector3Int(x, y, 0);
        Vector2Int positionFromCenter = new Vector2Int(x - islandCenter.x, y - islandCenter.y);
        float distanceToCenter = positionFromCenter.magnitude;

        float noiseValue1 = Mathf.PerlinNoise((float)(x + seed) / noiseScale, (float)(y + seed) / noiseScale);
        float noiseValue2 = Mathf.PerlinNoise((float)(x + 2 * seed) / (3 * noiseScale), (float)(y + 2 * seed) / (3 * noiseScale));
        float noiseValue = (2 * noiseValue1 * LandformScale + noiseValue2) / 3;

        float swampNoiseValue = Mathf.PerlinNoise((float)(x + 200 + seed) / (noiseScale * 3 / 2), (float)(y + 200 + seed) / (noiseScale * 3 / 2));
        float treeNoiseValue = Mathf.PerlinNoise((float)(x + 100 + seed) / (2 * noiseScale), (float)(y + 100 + seed) / (2 * noiseScale));

        float distortedRadius = IslandRadius * (1 + (noiseValue - 0.5f) * 0.5f);

        swampNoise = swampNoiseValue - swampNoiseValue * (distanceToCenter * 0.3f / distortedRadius);
        treeNoise = Mathf.Pow(treeNoiseValue - 0.08f, 3) * ResourceRate;
        biome = noiseValue - noiseValue * Mathf.Pow((distanceToCenter / distortedRadius), (float)(1.2f));
        biome = Mathf.Clamp01(biome);
    }

    private void LateGWInitialization()
    {
        biomeWrappedList = new FloatListWrapper(biomeList);
        swamp.CreateSwampBorders();
        tilemapResourcesController.Initialize();
        tilemapBiomesController.Initialize();
        uiManager.CloseAllPopUp();
        tilemapBiomesController.CategorizeIslands();
    }

    private void EarlyGWInitialization()
    {
        if (SeedString == "" || SeedString == null)
        {
            seed = random.Next(0, 1001);
        }
        else
        {
            seed = SeedFromText.GenerateSeedFromText(SeedString);
        }
        biomeList.Clear();
        tilemapControllers.ForEach(controller => controller.ClearAllTiles());
        MapWidth = (int)(IslandRadius * 2.5);
        tilemapBiomesController.tilemapChars = new char[MapWidth, MapWidth];
        islandCenter = new Vector2Int(MapWidth / 2, MapWidth / 2);
        Camera.main.transform.position = new Vector3(islandCenter.x, islandCenter.y, -10);
    }

    public Vector2Int GetIslandCenter() { return islandCenter; }
    public Swamp GetSwamp() { return swamp; }

}