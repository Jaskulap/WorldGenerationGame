using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Manages swamp-related operations, such as creating swamp borders through flood-fill algorithm.
public class Swamp : MonoBehaviour
{
    private bool[,] tilemapSwampChecked;
    int mapWidth;

    [SerializeField] private Tilemap tilemapBiomes;
    [SerializeField] private WorldGeneration worldGeneration;
    [SerializeField] private TilemapBiomesController tilemapBiomesController;

    [SerializeField] private Tile swampBorderTile;


    // Creates swamp borders for each swamp region on the map.
    public void CreateSwampBorders()
    {
        mapWidth = worldGeneration.MapWidth;
        tilemapSwampChecked = new bool[mapWidth, mapWidth];

        // Initialize the array to false.
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapWidth; y++)
            {
                tilemapSwampChecked[x, y] = false;
            }
        }

        // Iterate through each tile to find swamp regions.
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapWidth; y++)
            {
                if (tilemapBiomesController.tilemapChars[x, y] == 'b' && tilemapSwampChecked[x, y] == false)
                {
                    // Start flood-fill to identify and mark the swamp region.
                    FloodFillSwamp(x, y);
                }
            }
        }

        Debug.Log("Using the new Swamp class");
    }

    // Struct to store two Vector2Int positions, old and current tile
    struct TwoVec2
    {
        public Vector2Int zapis;
        public Vector2Int aktualny;
    }

    // Implements flood-fill algorithm to identify and mark swamp regions.
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

            // Skip if the position is out of bounds or already checked.
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapWidth || tilemapSwampChecked[x, y] == true)
            {
                continue;
            }

            // Check if the current position is not part of the swamp, then mark saved tile as border.
            if (tilemapBiomesController.tilemapChars[x, y] != 'b')
            {
                tilemapBiomes.SetTile(new Vector3Int(zapis.x, zapis.y, 0), swampBorderTile);
                tilemapBiomesController.tilemapChars[zapis.x, zapis.y] = 'b';
                tilemapSwampChecked[zapis.x, zapis.y] = true;
                continue;
            }

            // Mark the current position as checked.
            tilemapSwampChecked[x, y] = true;

            // Check neighbouring positions.
            queue.Enqueue(new TwoVec2() { zapis = new Vector2Int(x, y), aktualny = new Vector2Int(x + 1, y) }); // Right
            queue.Enqueue(new TwoVec2() { zapis = new Vector2Int(x, y), aktualny = new Vector2Int(x - 1, y) }); // Left
            queue.Enqueue(new TwoVec2() { zapis = new Vector2Int(x, y), aktualny = new Vector2Int(x, y + 1) }); // Up
            queue.Enqueue(new TwoVec2() { zapis = new Vector2Int(x, y), aktualny = new Vector2Int(x, y - 1) }); // Down
        }
    }
}
