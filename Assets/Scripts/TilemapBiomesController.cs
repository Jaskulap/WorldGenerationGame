using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBiomesController : BaseTilemapController
{
    public Tilemap tilemap;

    public char[] walkableChars = { 'g', 's', 'b', 'B' };

    public int[,] tilemapIslandNumber;
    public int[,] movementCosts;

    public bool categorized = false;

    public TilesManager tileManager;
    public WorldGenerationConditions worldGenerationConditions;
    public WorldGeneration worldGeneration;

    // Override method to set generated tiles based on biome and conditions
    public override void SetGeneratedTile(float biome, float swampNoise, float treeNoise, Vector3Int position)
    {
        int x = position.x;
        int y = position.y;
        int indexTile = Mathf.Clamp((int)(biome * 100), 0, 100);
        tilemapChars[x, y] = tileManager.tileList[indexTile].biome;
        tilemap.SetTile(position, tileManager.tileList[indexTile].tile);

        if (worldGenerationConditions.IsGrassBiome(biome))
        {
            if (worldGenerationConditions.IsSwampBiome(biome, swampNoise))
            {
                worldGenerationConditions.SetSwampTile(x, y, position);
            }
        }
    }

    // Override method to delete a tile at specified coordinates
    // Override method to delete a tile at specified coordinates
    public override void DeleteTile(Vector3 cords)
    {
        try
        {
            int x = Mathf.RoundToInt(cords.x);
            int y = Mathf.RoundToInt(cords.y);

            if (!IsCellInBorders(new Vector3Int(x, y, 0)))
            {
                throw new ArgumentException("Invalid position.");
            }

            tilemapChars[x, y] = BiomeType.WATER;
            tilemap.SetTile(new Vector3Int(x, y, 0), tileManager.tileList[0].tile);
        }
        catch (ArgumentException e)
        {
            Debug.LogError("Error deleting tile: " + e.Message);
        }
    }

    public override void ClearAllTiles()
    {

        tilemapChars = new char[gridSize, gridSize];
        tilemap.ClearAllTiles();

    }

    // Override method to get the biome type at specified coordinates
    public override char GetTileOnCords(Vector3 cellPosition)
    {
        try
        {
            int x = Mathf.RoundToInt(cellPosition.x);
            int y = Mathf.RoundToInt(cellPosition.y);

            Vector3Int cell = new Vector3Int(x, y, 0);

            // Check if cell is within valid range
            if (IsCellInBorders(cell))
            {
                throw new ArgumentException("Invalid cell position.");
            }

            x = Mathf.Clamp(x, 0, tilemapChars.GetLength(0) - 1);
            y = Mathf.Clamp(y, 0, tilemapChars.GetLength(1) - 1);

            return tilemapChars[x, y];
        }
        catch (ArgumentException e)
        {
            Debug.LogError("Error getting tile at coordinates: " + e.Message);
            return BiomeType.WATER; // Or another default value as needed
        }
    }

    // Set island number for every tile on the map.
    public void CategorizeIslands()
    {
        // Initialize island numbers to zero.
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                tilemapIslandNumber[x, y] = 0;
            }
        }

        int index = 1;
        // Iterate through the grid to find unprocessed land tiles and start flood fill.
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (tilemapIslandNumber[x, y] == 0 && tilemapChars[x, y] != BiomeType.MOUNTAIN && tilemapChars[x, y] != BiomeType.WATER)
                {
                    FloodFill(x, y, index);
                    index++;
                }
            }
        }

        categorized = true;

        // Update movement costs based on categorized islands.
        UpdateMovementCosts();
    }
    // Implements flood-fill algorithm to identify and mark independent island regions
    public void FloodFill(int startX, int startY, int index)
    {

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));


        while (queue.Count > 0)
        {
            Vector2Int position = queue.Dequeue();
            int x = position.x;
            int y = position.y;

            // Skip if the position is out of bounds or if it's water, mountain, or already categorized.
            if (x < 0 || x >= gridSize || y < 0 || y >= gridSize ||
                tilemapChars[x, y] == BiomeType.WATER || tilemapChars[x, y] == BiomeType.MOUNTAIN || tilemapIslandNumber[x, y] != 0)
            {
                continue;
            }

            // Set the island number for the current position.
            tilemapIslandNumber[x, y] = index;

            // Enqueue adjacent positions for further exploration.
            queue.Enqueue(new Vector2Int(x + 1, y)); // Right
            queue.Enqueue(new Vector2Int(x - 1, y)); // Left
            queue.Enqueue(new Vector2Int(x, y + 1)); // Up
            queue.Enqueue(new Vector2Int(x, y - 1)); // Down
        }
    }

    public void Initialize()
    {
        // Initialize grid size and island number array.
        gridSize = tilemapChars.GetLength(0);
        tilemapIslandNumber = new int[gridSize, gridSize];
    }


    public void UpdateMovementCosts()
    {
        movementCosts = new int[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                char tile = tilemapChars[x, y];
                if (IsTileWalkable(tile))
                {
                    movementCosts[x, y] = GetMovementCost(tile);
                }
                else
                {
                    movementCosts[x, y] = int.MaxValue;
                }
            }
        }
    }



    public int GetIslandNrOnCords(Vector3 cellPosition)
    {
        int x = Mathf.RoundToInt(cellPosition.x);
        int y = Mathf.RoundToInt(cellPosition.y);
        return tilemapIslandNumber[x, y];
    }

    public bool IsSameIsland(Vector3 cords1, Vector3 cords2)
    {
        return GetIslandNrOnCords(cords1) == GetIslandNrOnCords(cords2);
    }


    public bool IsTileWalkable(char tile)
    {
        foreach (char walkableTile in walkableChars)
        {
            if (tile == walkableTile)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsCellValid(Vector3Int cell)
    {
        char tile = tilemapChars[cell.x, cell.y];
        return IsCellInBorders(cell) && tile != BiomeType.WATER && tile != BiomeType.MOUNTAIN;
    }

    public bool IsCellInBorders(Vector3Int cell)
    {
        return cell.x >= 0 && cell.x < gridSize && cell.y >= 0 && cell.y < gridSize;
    }

    public int GetMovementCost(char tile)
    {
        if (tile == BiomeType.GRASS) return 1;
        if (tile == BiomeType.SAND) return 5;
        if (tile == BiomeType.SWAMP || tile == BiomeType.SWAMP_BORDER) return 10; // bagno
        return int.MaxValue;
    }

    public float GetSpeedModifierOnTile(char tile)
    {
        if (tile == BiomeType.GRASS) return 1f;
        if (tile == BiomeType.SAND) return 0.5f;
        if (tile == BiomeType.SWAMP || tile == BiomeType.SWAMP_BORDER) return 0.2f;
        return 1f;
    }
}
