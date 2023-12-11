using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
public class TilemapResourcesController : MonoBehaviour
{
    public Tilemap tilemap;
    private Vector3Int gridSize;
    public int treeCount;
    public int stoneCount;
    public List<Vector3> occupiedResourcesTree = new List<Vector3>();
    public List<Vector3> occupiedResourcesStone = new List<Vector3>();

    public Tile stoneTile;
    public Tile treeTile;

    public TilemapBiomesController tilemapBiomesController;

    public void Initialize()
    {
        treeCount = 0;
        stoneCount = 0;

        gridSize = tilemap.size;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
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

    public bool IsResourceAvaliable(string resource)
    {
        switch (resource)
        {
            case ResourceType.TREE:
                if(treeCount - occupiedResourcesTree.Count <= 0)
                    return false;
                break;
            case ResourceType.STONE:
                if(stoneCount - occupiedResourcesStone.Count <= 0)
                    return false;
                break;
        }
        return true;
    }

    public Vector3? GetNearestResourceCords(Vector3 startingCords, string resourceName)
    {
        if (!IsResourceAvaliable(resourceName))
        {
            return null;
        }
       
        Vector3? nearestResourceCords = null;

        bool found = false;
        int iterations = 0;
        float shortestDistance = float.MaxValue;
        float searchRadius = (float)gridSize.x * 0.1f; //10% mapy

        Vector3Int startingCellPosition = tilemap.WorldToCell(startingCords);

        BoundsInt searchBounds = new BoundsInt(
            startingCellPosition - new Vector3Int(Mathf.FloorToInt(searchRadius), Mathf.FloorToInt(searchRadius), 0),
            new Vector3Int(Mathf.CeilToInt(searchRadius * 2), Mathf.CeilToInt(searchRadius * 2), 1)
        );

        while (!found && iterations < 5)
        {
            iterations++;
            foreach (Vector3Int cellPosition in searchBounds.allPositionsWithin) // szukanie w bounds
            {
                TileBase tile = tilemap.GetTile(cellPosition);
                if (tile != null && tile.name == resourceName && !IsResourceOccupied(cellPosition) && tilemapBiomesController.CompareIslands(cellPosition, startingCords))
                {
                    Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
                    float distance = Vector3.Distance(startingCords, worldPosition);

                    found = true;
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestResourceCords = worldPosition;
                    }
                }
            }
            searchRadius *= 2;
            searchBounds = new BoundsInt(
                startingCellPosition - new Vector3Int(Mathf.FloorToInt(searchRadius), Mathf.FloorToInt(searchRadius), 0),
                new Vector3Int(Mathf.CeilToInt(searchRadius * 2), Mathf.CeilToInt(searchRadius * 2), 1)
            );
        }

        if (!found)
        {
            return null;
        }

        switch (resourceName)
        {
            case ResourceType.TREE:
                occupiedResourcesTree.Add(nearestResourceCords.Value);
                break;
            case ResourceType.STONE:
                occupiedResourcesStone.Add(nearestResourceCords.Value);
                break;
        }

        return nearestResourceCords;
    }

    public string GetResourceTypeOnCords(Vector3 cords)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(cords);
        if (tilemap.GetTile(cellPosition) != null)
        {
            string tile = tilemap.GetTile(cellPosition).name;
            switch (tile)
            {
                case "treeTile":
                    return ResourceType.TREE;
                case "stoneTile":
                    return ResourceType.STONE;
                default:
                    return null;
            }
        }
        else
            return null;
    }

    public void DeleteResource(Vector3 cords)
    {
        string resource = GetResourceTypeOnCords(cords);
        LowerResourceCount(resource);
        ReleaseOccupiedResource(cords);

        Vector3Int cellPosition = tilemap.WorldToCell(cords);
        tilemap.SetTile(cellPosition, null);
    }

    public void LowerResourceCount(string resource)
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
        string resource = GetResourceTypeOnCords(cords);

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

    public void DeleteSpecificResource(Vector3Int cords, string type)
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
        if (GetResourceTypeOnCords(cellPosition) == ResourceType.TREE && occupiedResourcesTree.Contains(cellPosition))
        {
            return true;
        }
        else if (GetResourceTypeOnCords(cellPosition) == ResourceType.STONE && occupiedResourcesStone.Contains(cellPosition))
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

    public void SetResourceTile(Vector3Int cellPosition, string resource)
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