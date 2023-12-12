using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

public class TilesManagerTests
{
    private TilesManager tilesManager;
    private GameObject tilesManagerObject;

    [SetUp]
    public void SetUp()
    {
        // Arrange: Create a new GameObject and add TilesManager component
        tilesManagerObject = new GameObject("TilesManagerTestObject");
        tilesManager = tilesManagerObject.AddComponent<TilesManager>();
    }

    [UnityTest]
    public IEnumerator TilesManager_PopulateTileList()
    {
        // Act: Wait for 1 second (example) to simulate asynchronous operation
        yield return new WaitForSeconds(1.0f);

        // Assert: Check if the tilesManager has populated tile lists
        Assert.NotNull(tilesManager.baseGrassTile);
        Assert.IsNotNull(tilesManager.tileList);
        Assert.IsNotNull(tilesManager.tileListSpecial);

        // Example: Assert that the tileList contains expected tiles
        Assert.AreEqual(101, tilesManager.tileList.Count); // Adjust the expected count based on your actual data
        Assert.AreEqual(BiomeType.WATER, tilesManager.tileList[0].biome);
        Assert.AreEqual(BiomeType.SAND, tilesManager.tileList[15].biome);
        Assert.AreEqual(BiomeType.GRASS, tilesManager.tileList[30].biome);
        Assert.AreEqual(BiomeType.MOUNTAIN, tilesManager.tileList[60].biome);

        // Example: Assert that the tileListSpecial contains expected tiles
        Assert.AreEqual(1, tilesManager.tileListSpecial.Count); // Adjust the expected count based on your actual data
        Assert.AreEqual(BiomeType.SWAMP, tilesManager.tileListSpecial[0].biome);
    }

    [Test]
    public void TilesManager_CreateTiles()
    {
        // Arrange
        TilesManager tilesManager = new TilesManager();
        List<TilesManager.Four> tileList = new List<TilesManager.Four>();

        // Act: Create tiles and populate the tileList
        tilesManager.CreateTiles(tileList, new Tile(), 4, 0.1f, 0.5f, 'g', true, false);

        // Assert: Check if the tileList is populated with the created tiles
        Assert.IsNotNull(tileList);
        Assert.AreEqual(4, tileList.Count);

        foreach (var tileInfo in tileList)
        {
            // Assert: Check each created tile in the tileList
            Assert.IsNotNull(tileInfo.tile);
            Assert.AreEqual('g', tileInfo.biome);
            // Add additional assertions based on your implementation
        }
    }
}
