using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TilemapBiomesController : MonoBehaviour
{
    public char[,] tilemapChars;
    public char[] walkableChars;
    public int gridSize;
    public int[,] tilemapIslandNumber;

    //public Tilemap tilemap; // do wyswietlania cordow tylko

    public int[,] movementCosts;

    private int categorizeCounter = 0;
    public bool categorized = false;

    public void CategorizeIslands()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                tilemapIslandNumber[x, y] = 0;
            }
        }

        int index = 1;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (tilemapIslandNumber[x, y] == 0 && tilemapChars[x, y] != 'm' && tilemapChars[x, y] != 'w')
                {
                    FloodFill(x, y, index);
                    // Debug.Log("indeks to" + index);
                    index++;

                }
            }
        }
        //Debug.Log("KONIEC KATEGORYZACJI WYSP!");
        categorized = true;

        UpdateMovementCosts();

    }
    public void Initialize()
    {
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

    public void FloodFill(int startX, int startY, int index)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));

        while (queue.Count > 0)
        {
            Vector2Int position = queue.Dequeue();
            int x = position.x;
            int y = position.y;

            if (x < 0 || x >= gridSize || y < 0 || y >= gridSize)
            {
                continue; // Wyjd�, je�li pozycja jest poza granicami
            }
            if (tilemapChars[x, y] == 'w' || tilemapChars[x, y] == 'm' || tilemapIslandNumber[x, y] != 0)
            {
                continue;
            }

            // Przypisz numer obszaru
            tilemapIslandNumber[x, y] = index;

            // Dodaj s�siednie pola do kolejki
            queue.Enqueue(new Vector2Int(x + 1, y)); // Prawo
            queue.Enqueue(new Vector2Int(x - 1, y)); // Lewo
            queue.Enqueue(new Vector2Int(x, y + 1)); // G�ra
            queue.Enqueue(new Vector2Int(x, y - 1)); // D�
        }
    }


    public int getIslandNrOnCords(Vector3 cellPosition)
    {
        int x = Mathf.RoundToInt(cellPosition.x);
        int y = Mathf.RoundToInt(cellPosition.y);
        return tilemapIslandNumber[x, y];
    }


    private void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        //     Debug.Log("Numer wyspy: " + tilemapIslandNumber[cellPosition.x, cellPosition.y] + " " + cellPosition);
        // }

        if (categorized)
        {
            categorizeCounter++;
            if (categorizeCounter >= 10)
            {
                categorized = false;
                categorizeCounter = 0;
            }
        }

    }

    public char getTileOnCords(Vector3Int cellPosition)
    {
        return (tilemapChars[cellPosition.x, cellPosition.y]);
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
        return cell.x >= 0 && cell.x < gridSize && cell.y >= 0 && cell.y < gridSize && tile != 'w' && tile != 'm';
    }

    public bool IsCellInBorders(Vector3Int cell)
    {
        return cell.x >= 0 && cell.x < gridSize && cell.y >= 0 && cell.y < gridSize;
    }

    public int GetMovementCost(char tile)
    {
        if (tile == 'g') return 1;
        if (tile == 's') return 5;
        if (tile == 'b' || tile == 'B') return 10; // bagno
        return int.MaxValue;
    }

    public float getSpeedModifierOnTile(char tile)  //predkosci trzeba dostosowac zeby byly odwrotnie proporcjonalne do kosztow przejscia, ale na razie jest tak zeby bylo widac zmiane
    {
        if (tile == 'g') return 1f;
        if (tile == 's') return 0.5f;
        if (tile == 'b' || tile == 'B') return 0.2f;
        return 1f;
    }

}
