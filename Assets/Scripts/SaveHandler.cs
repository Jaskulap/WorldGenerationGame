using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;

// Handles saving and loading game data including tilemap information.
public class SaveHandler : MonoBehaviour
{
    public Tilemap tilemapBiomes;
    public WorldGeneration worldGeneration;

    public TilemapBiomesController tilemapBiomesController;
    public TilemapResourcesController tilemapResourcesController;
    public UIManager uiManager;
    public TMP_InputField filenameInput;

    public TilesManager tileManager;

    // Dictionary to store references to different tilemaps in the scene.
    Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    [SerializeField] BoundsInt bounds;

    public string filename;

    // Initialization method called when the script is first loaded.
    private void Start()
    {
        InitTilemaps();
    }

    // Event handler for world selection.
    public void OnWorldPick(string filename)
    {
        Debug.Log("OnWorldPick");
        this.filename = filename;
    }

    // Event handler for filename input change.
    public void OnFilenameChange()
    {
        filename = filenameInput.text;
    }

    // Initializes the dictionary with references to different tilemaps in the scene.
    private void InitTilemaps()
    {
        int index = 0;
        Tilemap[] maps = FindObjectsOfType<Tilemap>();
        foreach (var map in maps)
        {
            index++;
            tilemaps.Add(map.name, map);
        }
    }

    // Saves the current state of the game.
    public void onSave()
    {
        //Debug.Log("SAVE TRY!");
        List<TilemapData> data = new List<TilemapData>();

        foreach (var mapObj in tilemaps)
        {
            TilemapData mapData = new TilemapData();
            mapData.key = mapObj.Key;
            BoundsInt boundsForThisMap = mapObj.Value.cellBounds;

            for (int x = boundsForThisMap.xMin; x <= boundsForThisMap.xMax; x++)
            {
                for (int y = boundsForThisMap.yMin; y <= boundsForThisMap.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase tile = mapObj.Value.GetTile(pos);

                    if (tile != null)
                    {
                        TileInfo ti = new TileInfo(tile, pos);
                        mapData.tiles.Add(ti);
                    }
                }
            }
            data.Add(mapData);
        }

        string folderPath = Application.persistentDataPath;
        Directory.CreateDirectory(folderPath + "/" + filename);
        FileHandler.SaveToJSON<TilemapData>(data, filename + "/" + filename + "TilemapData.json");
        //worldGeneration.biomeWrappedList = new FloatListWrapper(worldGeneration.biomeList);
        FileHandler.SaveToJSON(worldGeneration.biomeWrappedList.floatList, filename + "/" + filename + "BiomeData.json");

        Debug.Log("SAVE END!");
        uiManager.CloseAllPopUp();
    }

    // Loads a previously saved game state.
    public void onLoad()
    {
        Stopwatch stopwatch = new Stopwatch();

        // Start measuring time
        stopwatch.Start();

        List<TilemapData> data;
        List<float> biomeData;
        try
        {
            data = FileHandler.ReadListFromJSON<TilemapData>(filename + "/" + filename + "TilemapData.json");
            biomeData = FileHandler.ReadListFromJSON<float>(filename + "/" + filename + "BiomeData.json");

            // Rest of your code for handling the loaded data
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError($"File not found: {ex.FileName}");
            return;
            // Handle the case where the file is not found (e.g., inform the user, log, etc.)
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
            return;
            // Handle other exceptions (e.g., file format issues, unexpected errors, etc.)
        }

        worldGeneration.biomeWrappedList = new FloatListWrapper(biomeData);
        worldGeneration.biomeList = biomeData;
        worldGeneration.MapWidth = (int)Mathf.Sqrt(biomeData.Count);
        tilemapBiomesController.tilemapChars = new char[worldGeneration.MapWidth, worldGeneration.MapWidth];

        foreach (var mapData in data)
        {
            if (!tilemaps.ContainsKey(mapData.key))
            {
                Debug.LogError("Something went wrong!");
                continue;
            }
            var map = tilemaps[mapData.key];

            map.ClearAllTiles();

            if (mapData.tiles != null && mapData.tiles.Count > 0)
            {
                int index = 0;
                if (mapData.key == "Tilemap Biomes")
                {
                    foreach (TileInfo tile in mapData.tiles)
                    {
                        map.SetTile(tile.position, tileManager.tileList[(int)(biomeData[index] * 100)].tile);
                        tilemapBiomesController.tilemapChars[tile.position.x, tile.position.y] = tileManager.tileList[(int)(biomeData[index] * 100)].biome;
                        index++;
                    }
                }
                else
                {
                    foreach (TileInfo tile in mapData.tiles)
                    {
                        map.SetTile(tile.position, tile.tile);
                    }
                }
            }
        }
        stopwatch.Stop();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        Debug.Log("Load Time: " + elapsedMilliseconds + " ms");

        tilemapResourcesController.Initialize();
        tilemapBiomesController.Initialize();
        uiManager.CloseAllPopUp();
    }
}

// Serializable class to hold tilemap data.
[Serializable]
public class TilemapData
{
    public string key;
    public List<TileInfo> tiles = new List<TileInfo>();
}

// Serializable class to hold information about a tile.
[Serializable]
public class TileInfo
{
    public TileBase tile;
    public Vector3Int position;

    public TileInfo(TileBase tile, Vector3Int pos)
    {
        this.tile = tile;
        position = pos;
    }
}
