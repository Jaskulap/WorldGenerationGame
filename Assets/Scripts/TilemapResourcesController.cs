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

    void Start()
    {
    }

    void Update()
    {
    }

    public Vector3? GetNearestResourceCords(Vector3 startingCords, string resourceName) // trzeba jeszcze dodac obsluge jak w ogole nie ma na mapie tego resourca
    {
        if (resourceName == "treeTile" && (treeCount - occupiedResourcesTree.Count <= 0))
        {
            return null;
        }
        else if (resourceName == "stoneTile" && (treeCount - occupiedResourcesTree.Count <= 0))
        {
            return null;
        }

        Vector3? nearestResourceCords = null;

        //    Debug.Log("szukam " + resourceName);

        bool found = false;
        int iterations = 0;
        float shortestDistance = float.MaxValue;
        float searchRadius = (float)gridSize.x * 0.1f; //10% mapy

        Vector3Int startingCellPosition = tilemap.WorldToCell(startingCords);

        int startingIslandNr = tilemapBiomesController.getIslandNrOnCords(startingCords);


        BoundsInt searchBounds = new BoundsInt(
            startingCellPosition - new Vector3Int(Mathf.FloorToInt(searchRadius), Mathf.FloorToInt(searchRadius), 0),
            new Vector3Int(Mathf.CeilToInt(searchRadius * 2), Mathf.CeilToInt(searchRadius * 2), 1)
        );

        while (!found && iterations < 5) // trzeba bedzie dostosowac do najwiekszej mapy; albo zrobic zmienna zalezna od promienia mapy;; bo bylo 3 i dla map powyzej polowy suwaka zostawial
        {//czy musi by� 6 iteracji - nie wystarcz� 4 lub 5??
            iterations++;
            foreach (Vector3Int cellPosition in searchBounds.allPositionsWithin) // szukanie w bounds
            {
                TileBase tile = tilemap.GetTile(cellPosition);
                if (tile != null && tile.name == resourceName && !IsResourceOccupied(cellPosition) && (startingIslandNr == tilemapBiomesController.getIslandNrOnCords(cellPosition)))
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

        if (resourceName == "treeTile")
        {
            occupiedResourcesTree.Add(nearestResourceCords.Value);
        }
        else if (resourceName == "stoneTile")
        {
            occupiedResourcesStone.Add(nearestResourceCords.Value);
        }

        return nearestResourceCords;
    }


    public string GetResourceTypeOnCords(Vector3 cords)
    {



        Vector3Int cellPosition = tilemap.WorldToCell(cords);
        if (tilemap.GetTile(cellPosition) != null)
            return tilemap.GetTile(cellPosition).name;
        else
            return null;
    }

    public void deleteResource(Vector3 cords) // to mozna przerobic zeby korzystalo z relase
    {
        Vector3Int cellPosition = tilemap.WorldToCell(cords);

        if (GetResourceTypeOnCords(cords) == "treeTile")
        {
            occupiedResourcesTree.Remove(cords);
            treeCount--;

        }
        else if (GetResourceTypeOnCords(cords) == "stoneTile")
        {
            occupiedResourcesStone.Remove(cords);
            stoneCount--;
        }

        tilemap.SetTile(cellPosition, null);
    }

    public void ReleaseOccupiedResource(Vector3 cords)
    {
        if (GetResourceTypeOnCords(cords) == "treeTile")
        {
            occupiedResourcesTree.Remove(cords);
        }
        else if (GetResourceTypeOnCords(cords) == "stoneTile")
        {
            occupiedResourcesStone.Remove(cords);
        }
    }

    public void deleteSpecificResource(Vector3Int cords, char type)
    {
        if (tilemap.GetTile(cords) == null)
        {
            return;
        }
        if (tilemap.GetTile(cords).name[0] != type)
        {
            return;
        }
        Vector3Int cellPosition = tilemap.WorldToCell(cords);
        tilemap.SetTile(cellPosition, null);

        ReleaseOccupiedResource(cords);

        if (type == 't')
        {
            treeCount--;
        }
        else if (type == 's')
        {
            stoneCount--;
        }
    }

    private bool IsResourceOccupied(Vector3 cellPosition)
    {
        if (GetResourceTypeOnCords(cellPosition) == "treeTile" && occupiedResourcesTree.Contains(cellPosition))
        {
            return true;
        }
        else if (GetResourceTypeOnCords(cellPosition) == "stoneTile" && occupiedResourcesStone.Contains(cellPosition))
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
        if (resource == 't')
        {
            tilemap.SetTile(cellPosition, treeTile);
            treeCount++;
        }
        else
        if (resource == BiomeType.SAND)
        {
            tilemap.SetTile(cellPosition, stoneTile);
            stoneCount++;
        }
    }

}