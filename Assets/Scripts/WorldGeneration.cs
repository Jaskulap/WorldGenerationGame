using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class WorldGeneration : MonoBehaviour
{
    public Tilemap tilemapBiomes;
    public Tilemap tilemapResources;
    public TilemapBiomesController tilemapBiomesController;
    public TilemapResourcesController tilemapResourcesController;
    [SerializeField]
    private Swamp swamp;
    public GameObject borderSquare;
    public Tile swampTile;
    public Tile waterStickTile;
    public Tile stoneTile;
    public Tile treeTile;
    private float landformScale = 1;
    private float resourceRate = 1;//0.66 - 1.5
    private string seedString = "";
    private int mapWidth = 100;
    private int islandRadius = 40;
    public int seed = 0;
    private Vector2Int islandCenter;
    private float noiseScale = 19f;
    public List<float> biomeList = new List<float>();
    public FloatListWrapper biomeWrappedList;
    Random random = new Random();
    public TilesManager tileManager;
    public UIManager uiManager;

    public int MapWidth { get => mapWidth; set => mapWidth = value; }
    public string SeedString { get => seedString; set => seedString = value; }
    public int IslandRadius { get => islandRadius; set => islandRadius = value; }
    public float LandformScale { get => landformScale; set => landformScale = value; }
    public float ResourceRate { get => resourceRate; set => resourceRate = value; }

    public void ClearWorld()
    {
        tilemapBiomes.ClearAllTiles();
        tilemapResources.ClearAllTiles();
        biomeList.Clear();
        tilemapBiomesController.tilemapChars = new char[MapWidth, MapWidth];
        GenerateWorld(empty: true);
    }

    public void GenerateWorld(bool empty)
    {
        EarlyGWInitialization();
        if (empty)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapWidth; y++)
                {
                    tilemapBiomes.SetTile(new Vector3Int(x, y, 0), tileManager.tileList[0].tile);
                    tilemapBiomesController.tilemapChars[x, y] = BiomeType.WATER;
                    biomeList.Add(0f);

                }
            }
        }
        else
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapWidth; y++)
                {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);
                    Vector2Int positionFromCenter = new Vector2Int(x - islandCenter.x, y - islandCenter.y);
                    float distanceToCenter = positionFromCenter.magnitude;

                    float noiseValue1 = Mathf.PerlinNoise((float)(x + seed) / noiseScale, (float)(y + seed) / noiseScale);
                    float noiseValue2 = Mathf.PerlinNoise((float)(x + 2 * seed) / (3 * noiseScale), (float)(y + 2 * seed) / (3 * noiseScale));
                    float noiseValue = (2 * noiseValue1 * LandformScale + noiseValue2) / 3;
                    float distortedRadius = IslandRadius * (1 + (noiseValue - 0.5f) * 0.5f);

                    float swampNoiseValue = Mathf.PerlinNoise((float)(x + 200 + seed) / (noiseScale * 3 / 2), (float)(y + 200 + seed) / (noiseScale * 3 / 2));
                    float treeNoiseValue = Mathf.PerlinNoise((float)(x + 100 + seed) / (2 * noiseScale), (float)(y + 100 + seed) / (2 * noiseScale));

                    float swampNoise = swampNoiseValue - swampNoiseValue * (distanceToCenter * 0.3f / distortedRadius);
                    float treeNoise = Mathf.Pow(treeNoiseValue - 0.08f, 3) * ResourceRate;

                    float biome = noiseValue - noiseValue * Mathf.Pow((distanceToCenter / distortedRadius), (float)(1.2f));
                    biome = Mathf.Clamp01(biome);
                    if (biome > 0.1f)
                    {
                        biome = biome * LandformScale;
                    }
                    biomeList.Add(biome);

                    int indexTile = Mathf.Clamp((int)(biome * 100), 0, 100);
                    tilemapBiomesController.tilemapChars[x, y] = tileManager.tileList[indexTile].biome;
                    tilemapBiomes.SetTile(cellPosition, tileManager.tileList[indexTile].tile);

                    if (biome > 0.2f && biome < 0.50f)//trawa
                    {
                        if (biome > 0.25 && biome < 0.40 && swampNoise > 0.62)
                        {
                            tilemapBiomesController.tilemapChars[x, y] = BiomeType.SWAMP;
                            tilemapBiomes.SetTile(cellPosition, swampTile);
                        }
                        if (UnityEngine.Random.value <= treeNoise)
                        {
                            if (tilemapBiomes.GetTile(cellPosition).name != "swampTile")
                            {
                                tilemapResources.SetTile(cellPosition, treeTile);
                            }
                            else
                            {
                                tilemapResources.SetTile(cellPosition, waterStickTile);
                            }
                        }
                        if (biome >= 0.49 && biome < 0.50 && UnityEngine.Random.value <= 0.50f * (ResourceRate * 0.9))
                        {
                            tilemapResources.SetTile(cellPosition, stoneTile);
                        }
                    }
                }
            }
        }
        LateGWInitialization();

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
        tilemapBiomes.ClearAllTiles();
        tilemapResources.ClearAllTiles();
        MapWidth = (int)(IslandRadius * 2.5);
        tilemapBiomesController.tilemapChars = new char[MapWidth, MapWidth];
        islandCenter = new Vector2Int(MapWidth / 2, MapWidth / 2);
        Camera.main.transform.position = new Vector3(islandCenter.x, islandCenter.y, -10);
    }

    public Vector2Int GetIslandCenter() { return islandCenter; }
    public Swamp GetSwamp() { return swamp; }

}

[Serializable]
public class FloatListWrapper
{
    public List<float> floatList;
    public FloatListWrapper(List<float> floatList)
    {
        this.floatList = floatList;
    }
}

