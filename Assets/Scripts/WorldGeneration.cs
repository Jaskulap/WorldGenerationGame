using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class WorldGeneration : MonoBehaviour
{
    public Tilemap tilemapBiomes;
    public Tilemap tilemapResources;


    public TilemapBiomesController tilemapBiomesController;
    public TilemapResourcesController tilemapResourcesController;

   
    public GameObject borderSquare;

    public Tile swampTile;
    public Tile swampBorderTile;

    public Tile waterStickTile;
    public Tile stoneTile;
    public Tile treeTile;

  
    public Slider islandRadiusSlider;
    public Slider landformSlider;
    private float landformScale = 1;
    public Slider resourceRateSlider;
    private float resourceRate = 1;//0.66 - 1.5
    public TMP_InputField seedStringInputField;
    private string seedString = "";

  
    public int mapWidth = 100;
    public int islandRadius = 40;
    public int seed = 0;
    private Vector2Int islandCenter;
    private float noiseScale = 19f;

    public List<float> biomeList = new List<float>();
    public FloatListWrapper biomeWrappedList;

    Random random;

    public TilesManager tileManager;
    public UIManager uiManager;
  
    private bool[,] tilemapSwampChecked;
    void Start()
    {
        random = new Random();
        islandRadiusSlider.value = islandRadius;
    }
    public void OnGenerateSlidersValueChanged()
    {
        islandRadius = (int)islandRadiusSlider.value;
        landformScale = (float)landformSlider.value;
        resourceRate = (float)resourceRateSlider.value;

    }
    public void OnSeedInputChange()
    {

        seedString = seedStringInputField.text;
    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
           

        }
        else
               if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CreateSwampBorders();
        }
    }

    public void CreateSwampBorders()
    {
        tilemapSwampChecked = new bool[mapWidth, mapWidth];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapWidth; y++)
            {
                tilemapSwampChecked[x, y] = false;
            }
        }
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapWidth; y++)
            {
                if (tilemapBiomesController.tilemapChars[x, y] == 'b' && tilemapSwampChecked[x, y] == false)
                {
                    //Debug.Log("Start Swamp Check at: " + x + " " + y);
                    FloodFillSwamp(x, y);

                }
            }
        }

    }
    struct TwoVec2
    {
        public Vector2Int zapis;
        public Vector2Int aktualny;
    }
    public void FloodFillSwamp(int startX, int startY)
    {
        Queue<TwoVec2> queue = new Queue<TwoVec2>();
        queue.Enqueue(new TwoVec2() { zapis = new Vector2Int(startX, startY), aktualny = new Vector2Int(startX, startY) });

        while (queue.Count > 0)
        {
            TwoVec2 positions = queue.Dequeue();
            Vector2Int position = positions.aktualny;
            Vector2Int zapis = positions.zapis;
            int x = position.x;
            int y = position.y;


            if (x < 0 || x >= mapWidth || y < 0 || y >= mapWidth || tilemapSwampChecked[x, y] == true)
            {
                continue; // Wyjd�, je�li pozycja jest poza granicami
            }
            if (tilemapBiomesController.tilemapChars[x, y] != 'b')
            {
                tilemapBiomes.SetTile(new Vector3Int(zapis.x, zapis.y, 0), swampBorderTile);
                tilemapBiomesController.tilemapChars[zapis.x, zapis.y] = 'b';
                tilemapSwampChecked[zapis.x, zapis.y] = true;
                continue;
            }
            tilemapSwampChecked[x, y] = true;

            // Dodaj s�siednie pola do kolejki
            queue.Enqueue(new TwoVec2() { zapis = new Vector2Int(x, y), aktualny = new Vector2Int(x + 1, y) }); // Prawo
            queue.Enqueue(new TwoVec2() { zapis = new Vector2Int(x, y), aktualny = new Vector2Int(x - 1, y) }); // Lewo
            queue.Enqueue(new TwoVec2() { zapis = new Vector2Int(x, y), aktualny = new Vector2Int(x, y + 1) }); // G�ra
            queue.Enqueue(new TwoVec2() { zapis = new Vector2Int(x, y), aktualny = new Vector2Int(x, y - 1) }); // D�
        }
    }

    public int GenerateSeedFromText(string text)//0-1000
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            int seed = BitConverter.ToInt32(hashBytes, 0);
            seed = seed % 1001;
            if (seed < 0) seed = -seed;
            return seed;
        }
    }

    public void ClearWorld()
    {
        tilemapBiomes.ClearAllTiles();
        tilemapResources.ClearAllTiles();
        biomeList.Clear();
        tilemapBiomesController.tilemapChars = new char[mapWidth, mapWidth];
        GenerateWorld(empty: true);
    }

    public void GenerateWorld(bool empty)
    {
        if (seedString == "" || seedString == null)
        {
            seed = random.Next(0, 1001);
        }
        else
        {
            seed = GenerateSeedFromText(seedString);
        }
     
        biomeList.Clear();
        tilemapBiomes.ClearAllTiles();
        tilemapResources.ClearAllTiles();
        Stopwatch stopwatch = new Stopwatch();

        // Rozpoczęcie pomiaru czasu
        stopwatch.Start();
        mapWidth = (int)(islandRadius * 2.5);
        tilemapBiomesController.tilemapChars = new char[mapWidth, mapWidth];

        islandCenter = new Vector2Int(mapWidth / 2, mapWidth / 2);
        Camera.main.transform.position = new Vector3(islandCenter.x, islandCenter.y, -10);
        if (empty)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapWidth; y++)
                {
                    tilemapBiomes.SetTile(new Vector3Int(x, y, 0), tileManager.tileList[0].tile);
                    tilemapBiomesController.tilemapChars[x, y] = 'w';
                    biomeList.Add(0f);

                }
            }
        }
        else
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapWidth; y++)
                {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);
                    Vector2Int positionFromCenter = new Vector2Int(x - islandCenter.x, y - islandCenter.y);
                    float distanceToCenter = positionFromCenter.magnitude;

                    float noiseValue1 = Mathf.PerlinNoise((float)(x + seed) / noiseScale, (float)(y + seed) / noiseScale);
                    float noiseValue2 = Mathf.PerlinNoise((float)(x + 2 * seed) / (3 * noiseScale), (float)(y + 2 * seed) / (3 * noiseScale));
                    float noiseValue = (2 * noiseValue1 * landformScale + noiseValue2) / 3;
                    float distortedRadius = islandRadius * (1 + (noiseValue - 0.5f) * 0.5f);

                    float swampNoiseValue = Mathf.PerlinNoise((float)(x + 200 + seed) / (noiseScale * 3 / 2), (float)(y + 200 + seed) / (noiseScale * 3 / 2));
                    float treeNoiseValue = Mathf.PerlinNoise((float)(x + 100 + seed) / (2 * noiseScale), (float)(y + 100 + seed) / (2 * noiseScale));

                    float swampNoise = swampNoiseValue - swampNoiseValue * (distanceToCenter * 0.3f / distortedRadius);
                    float treeNoise = Mathf.Pow(treeNoiseValue - 0.08f, 3) * resourceRate;

                    float biome = noiseValue - noiseValue * Mathf.Pow((distanceToCenter / distortedRadius), (float)(1.2f));
                    biome = Mathf.Clamp01(biome);
                    if (biome > 0.1f)
                    {
                        biome = biome * landformScale;
                    }
                    biomeList.Add(biome);



                    int indexTile = Mathf.Clamp((int)(biome * 100), 0, 100);
                    tilemapBiomesController.tilemapChars[x, y] = tileManager.tileList[indexTile].biome;
                    tilemapBiomes.SetTile(cellPosition, tileManager.tileList[indexTile].tile);

                    if (biome > 0.2f && biome < 0.50f)//trawa
                    {
                        if (biome > 0.25 && biome < 0.40 && swampNoise > 0.62)
                        {
                            tilemapBiomesController.tilemapChars[x, y] = 'b';
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
                        if (biome >= 0.49 && biome < 0.50 && UnityEngine.Random.value <= 0.50f * (resourceRate * 0.9))
                        {
                            tilemapResources.SetTile(cellPosition, stoneTile);
                        }
                    }
                }
            }
        }
        biomeWrappedList = new FloatListWrapper(biomeList);
        stopwatch.Stop();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        Debug.Log("Czas GenerateWorld: " + elapsedMilliseconds + " ms");
        CreateSwampBorders();
        tilemapResourcesController.Initialize();
        tilemapBiomesController.Initialize();
        uiManager.CloseAllPopUp();
        tilemapBiomesController.CategorizeIslands();

    }

    public Vector2Int GetIslandCenter()
    {
        return islandCenter;
    }


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

