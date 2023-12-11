using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BiomeType
{
    public const char WATER = 'w';
    public const char GRASS = 'g';
    public const char SAND = 's';
    public const char SWAMP = 'b';
    public const char SWAMP_BORDER = 'B';
    public const char MOUNTAIN = 'm';
}

public class TilesManager : MonoBehaviour
{
    public struct Four
    {
        public Tile tile;
        public char biome;
        public bool deleteVege;
        public bool deleteRest;
    }
    public Tile baseWaterTile;
    public Tile baseSandTile;
    public Tile baseGrassTile;
    public Tile baseMountainTile;
    public Tile baseSnowTile;
    public Tile baseSwampTile;
    public Tile baseSwampBorderTile;

    public List<Four> tileList;
    public List<Four> tileListSpecial;
    void Awake()
    {
        tileList = new List<Four>();
        CreateTiles(tileList, baseWaterTile, 10, 0.025f, 0.75f, BiomeType.WATER, true, true);
        CreateTiles(tileList, baseSandTile, 10, 0.02f, 0.80f, BiomeType.SAND, true, false);
        CreateTiles(tileList, baseGrassTile, 30, -0.022f, 1f, BiomeType.GRASS, false, false);
        CreateTiles(tileList, baseMountainTile, 31, +0.015f, 0.5f, BiomeType.MOUNTAIN, true, true);
        CreateTiles(tileList, baseSnowTile, 20, 0.01f, 0.8f, BiomeType.MOUNTAIN, true, true);

        tileListSpecial = new List<Four>();
        CreateTiles(tileListSpecial, baseSwampTile, 1, 0, 1f, BiomeType.SWAMP, true, true);

    }
    public void CreateTiles(List<Four> tileList, Tile baseTile, int howMany, float colorChange, float startColor, char biomee, bool deleteVegetation, bool deleteeRest)
    {
        Color currentColor = new Color(startColor, startColor, startColor);
        for (int i = 0; i < howMany; i++)
        {
            Tile newTile = new Tile(); 
            newTile.sprite = baseTile.sprite;

            newTile.name = baseTile.name + i;
            newTile.color = currentColor;
            currentColor += new Color(colorChange, colorChange, colorChange);
            Four newFour = new Four() { tile = newTile, biome = biomee, deleteVege = deleteVegetation, deleteRest = deleteeRest };
            tileList.Add(newFour);
        }
    }

}
