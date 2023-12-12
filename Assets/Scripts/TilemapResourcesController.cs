using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
public class TilemapResourcesController : BaseTilemapController
{
    public Tilemap tilemap;

    public int treeCount = 0;
    public int stoneCount = 0;
    public List<Vector3> occupiedResourcesTree = new List<Vector3>();
    public List<Vector3> occupiedResourcesStone = new List<Vector3>();

    public Tile stoneTile;
    public Tile treeTile;
    public Tile waterStickTile;

    public TilemapBiomesController tilemapBiomesController;
    public WorldGenerationConditions worldGenerationConditions;
    public WorldGeneration worldGeneration;

    public void Start()
    {
        stoneTile = Resources.Load<Tile>("stoneTile");
        treeTile = Resources.Load<Tile>("treeTile");
        gridSize = tilemap.size.x;
    }

    // Override method to set generated tile based on biome conditions
    public override void SetGeneratedTile(float biome, float swampNoise, float treeNoise, Vector3Int position)
    {
        int x = position.x;
        int y = position.y;
        if (worldGenerationConditions.IsGrassBiome(biome))
        {
            if (worldGenerationConditions.ShouldPlaceTreeOrWaterStick(treeNoise, position))
            {
                worldGenerationConditions.PlaceTreeOrWaterStick(x, y, position);
            }

            if (worldGenerationConditions.ShouldPlaceStone(biome, worldGeneration.ResourceRate))
            {
                worldGenerationConditions.PlaceStone(x, y, position);
            }
        }
    }
    public void SetTile(Vector3Int cords, char type)
    {
        if (type == ResourceType.TREE)
        {
            Debug.Log("STAWIAM DRZEWO");
            tilemap.SetTile(cords, treeTile);
            treeCount++;
        }
        else
        if (type == ResourceType.STONE)
        {
            Debug.Log("STAWIAM KAMIEN");
            tilemap.SetTile(cords, stoneTile);
            stoneCount++;
        }

    }
    // Override method to delete a tile at specified coordinates
    public override void DeleteTile(Vector3 cords)
    {
        char resource = GetTileOnCords(cords);
        LowerResourceCount(resource);
        ReleaseOccupiedResource(cords);

        Vector3Int cellPosition = tilemap.WorldToCell(cords);
        tilemap.SetTile(cellPosition, null);
    }

    public override void ClearAllTiles()
    {
        treeCount = 0;
        stoneCount = 0;
        tilemap.ClearAllTiles();
    }

    // Override method to get the resource type at specified coordinates
    public override char GetTileOnCords(Vector3 cords)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(cords);
        if (tilemap.GetTile(cellPosition) != null)
        {
            Debug.Log(cellPosition);
            string tile = tilemap.GetTile(cellPosition).name;
            Debug.Log("TILE NAME: " + tile);
            switch (tile)
            {
                case "treeTile":
                    return ResourceType.TREE;
                case "stoneTile":
                    return ResourceType.STONE;
                default:
                    return ResourceType.NONE;
            }
        }
        else
            return ResourceType.NONE;
    }

    // Initialize method to count existing tree and stone tiles
    public void Initialize()
    {
        treeCount = 0;
        stoneCount = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile == treeTile)
                {
                    treeCount++;
                }
                else if (tile == stoneTile)
                {
                    stoneCount++;
                }
            }
        }
    }

    public bool IsResourceAvaliable(char resource)
    {
        switch (resource)
        {
            case ResourceType.TREE:
                if (treeCount - occupiedResourcesTree.Count <= 0)
                    return false;
                break;
            case ResourceType.STONE:
                if (stoneCount - occupiedResourcesStone.Count <= 0)
                    return false;
                break;
        }
        return true;
    }

    //public Vector3? GetNearestResourceCords(Vector3 startingCords, char resourceName)
    //{
    //    if (!IsResourceAvaliable(resourceName))
    //    {
    //        return null;
    //    }

    //    Vector3? nearestResourceCords = null;

    //    bool found = false;
    //    int iterations = 0;
    //    float shortestDistance = float.MaxValue;
    //    float searchRadius = (float)gridSize.x * 0.1f; //10% mapy

    //    Vector3Int startingCellPosition = tilemap.WorldToCell(startingCords);

    //    BoundsInt searchBounds = new BoundsInt(
    //        startingCellPosition - new Vector3Int(Mathf.FloorToInt(searchRadius), Mathf.FloorToInt(searchRadius), 0),
    //        new Vector3Int(Mathf.CeilToInt(searchRadius * 2), Mathf.CeilToInt(searchRadius * 2), 1)
    //    );

    //    while (!found && iterations < 5)
    //    {
    //        iterations++;
    //        foreach (Vector3Int cellPosition in searchBounds.allPositionsWithin) // szukanie w bounds
    //        {
    //            TileBase tile = tilemap.GetTile(cellPosition);
    //            if (tile != null && tile.name == resourceName && !IsResourceOccupied(cellPosition) && tilemapBiomesController.IsSameIsland(cellPosition, startingCords))
    //            {
    //                Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
    //                float distance = Vector3.Distance(startingCords, worldPosition);

    //                found = true;
    //                if (distance < shortestDistance)
    //                {
    //                    shortestDistance = distance;
    //                    nearestResourceCords = worldPosition;
    //                }
    //            }
    //        }
    //        searchRadius *= 2;
    //        searchBounds = new BoundsInt(
    //            startingCellPosition - new Vector3Int(Mathf.FloorToInt(searchRadius), Mathf.FloorToInt(searchRadius), 0),
    //            new Vector3Int(Mathf.CeilToInt(searchRadius * 2), Mathf.CeilToInt(searchRadius * 2), 1)
    //        );
    //    }

    //    if (!found)
    //    {
    //        return null;
    //    }

    //    switch (resourceName)
    //    {
    //        case ResourceType.TREE:
    //            occupiedResourcesTree.Add(nearestResourceCords.Value);
    //            break;
    //        case ResourceType.STONE:
    //            occupiedResourcesStone.Add(nearestResourceCords.Value);
    //            break;
    //    }

    //    return nearestResourceCords;
    //}

    public void LowerResourceCount(char resource)
    {
        switch (resource)
        {
            case ResourceType.TREE:
                treeCount--;
                break;
            case ResourceType.STONE:
                stoneCount--;
                break;
        }
    }

    public void ReleaseOccupiedResource(Vector3 cords)
    {
        char resource = GetTileOnCords(cords);

        switch (resource)
        {
            case ResourceType.TREE:
                occupiedResourcesTree.Remove(cords);
                break;
            case ResourceType.STONE:
                occupiedResourcesStone.Remove(cords);
                break;
        }
    }

    public void DeleteSpecificResource(Vector3Int cords, char type)
    {
        if (tilemap.GetTile(cords) == null)
        {
            return;
        }

        Vector3Int cellPosition = tilemap.WorldToCell(cords);
        tilemap.SetTile(cellPosition, null);

        ReleaseOccupiedResource(cords);
        LowerResourceCount(type);
    }

    private bool IsResourceOccupied(Vector3 cellPosition)
    {
        if (GetTileOnCords(cellPosition) == ResourceType.TREE && occupiedResourcesTree.Contains(cellPosition))
        {
            return true;
        }
        else if (GetTileOnCords(cellPosition) == ResourceType.STONE && occupiedResourcesStone.Contains(cellPosition))
        {
            return true;
        }
        return false;
    }

    public bool IsCellEmpty(Vector3 cellPosition)
    {
        Vector3Int cell = tilemap.WorldToCell(cellPosition);
        return (tilemap.GetTile(cell) == null);
    }

    public void SetResourceTile(Vector3Int cellPosition, char resource)
    {
        switch (resource)
        {
            case ResourceType.TREE:
                tilemap.SetTile(cellPosition, treeTile);
                treeCount++;
                break;
            case ResourceType.STONE:
                tilemap.SetTile(cellPosition, stoneTile);
                stoneCount++;
                break;
        }
    }

}