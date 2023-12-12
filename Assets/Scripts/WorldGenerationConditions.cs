using UnityEngine;
using UnityEngine.Tilemaps;

// Manages conditions and actions related to world generation.
public class WorldGenerationConditions : MonoBehaviour
{
    public TilemapBiomesController tilemapBiomesController;
    public Tilemap tilemapBiomes;
    public Tilemap tilemapResources;
    public Tile swampTile;
    public Tile treeTile;
    public Tile waterStickTile;
    public Tile stoneTile;

    // Checks if the given biome value corresponds to a grass biome.
    public bool IsGrassBiome(float biome)
    {
        return biome > 0.2f && biome < 0.50f;
    }

    // Checks if the given biome and swamp noise values correspond to a swamp biome.
    public bool IsSwampBiome(float biome, float swampNoise)
    {
        return biome > 0.25 && biome < 0.40 && swampNoise > 0.62;
    }

    // Sets the swamp tile at the specified position and updates the biome map.
    public void SetSwampTile(int x, int y, Vector3Int cellPosition)
    {
        tilemapBiomesController.tilemapChars[x, y] = BiomeType.SWAMP;
        tilemapBiomes.SetTile(cellPosition, swampTile);
    }

    // Determines if a tree or water stick should be placed based on the specified tree noise.
    public bool ShouldPlaceTreeOrWaterStick(float treeNoise, Vector3Int cellPosition)
    {
        return UnityEngine.Random.value <= treeNoise;
    }

    // Places a tree or water stick at the specified position based on biome conditions.
    public void PlaceTreeOrWaterStick(int x, int y, Vector3Int cellPosition)
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

    // Determines if a stone should be placed based on biome and resource rate conditions.
    public bool ShouldPlaceStone(float biome, float resourceRate)
    {
        return biome >= 0.49 && biome < 0.50 && UnityEngine.Random.value <= 0.50f * (resourceRate * 0.9);
    }

    // Places a stone at the specified position.
    public void PlaceStone(int x, int y, Vector3Int cellPosition)
    {
        tilemapResources.SetTile(cellPosition, stoneTile);
    }
}
