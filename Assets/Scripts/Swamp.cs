using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Swamp : MonoBehaviour
{
    private bool[,] tilemapSwampChecked;
    int mapWidth;
    [SerializeField]
    private Tilemap tilemapBiomes;
    [SerializeField]
    private WorldGeneration worldGeneration;
    [SerializeField]
    private TilemapBiomesController tilemapBiomesController;

    [SerializeField]
    private Tile swampBorderTile;
    void Start()
    {

    }


    void Update()
    {

    }
    public void CreateSwampBorders()
    {
        mapWidth = worldGeneration.MapWidth;
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
        Debug.Log("Uzywam nowej klasy Swamp");

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

}
