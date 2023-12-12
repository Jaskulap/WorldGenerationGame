using UnityEngine;

public abstract class BaseTilemapController : MonoBehaviour
{
    public char[,] tilemapChars;
    public int gridSize;

    public abstract void SetGeneratedTile(float biome, float swampNoise, float treeNoise, Vector3Int position);

    public abstract void DeleteTile(Vector3 cords);

    public abstract void ClearAllTiles();

    public abstract char GetTileOnCords(Vector3 cords);

}
