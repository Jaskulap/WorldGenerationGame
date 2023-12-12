using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapResourcesControllerTests
{
    [Test]
    public void TilemapResourcesController_ClearAllTiles_ShouldRemoveAllTilesAndResetCounts()
    {
        // Arrange
        GameObject tilemapResourcesControllerObject = new GameObject("TilemapResourcesControllerTestObject");
        TilemapResourcesController tilemapResourcesController = tilemapResourcesControllerObject.AddComponent<TilemapResourcesController>();
        tilemapResourcesController.tilemap = tilemapResourcesControllerObject.AddComponent<Tilemap>();
        tilemapResourcesController.treeTile = Resources.Load<Tile>("treeTile");
        tilemapResourcesController.stoneTile = Resources.Load<Tile>("stoneTile");

        // Act: Set initial tiles
        Vector3Int treePosition = new Vector3Int(0, 0, 0);
        Vector3Int stonePosition = new Vector3Int(1, 1, 0);
        tilemapResourcesController.SetTile(treePosition, 't');
        tilemapResourcesController.SetTile(stonePosition, 's');

        Vector3Int treePosition2 = new Vector3Int(2, 2, 0);
        Vector3Int stonePosition2 = new Vector3Int(3, 3, 0);
        tilemapResourcesController.SetTile(treePosition2, 't');
        tilemapResourcesController.SetTile(stonePosition2, 's');

        // Assert: Check initial conditions
        Assert.IsNotNull(tilemapResourcesController.tilemap.GetTile(treePosition));
        Assert.IsNotNull(tilemapResourcesController.tilemap.GetTile(treePosition2));
        Assert.IsNotNull(tilemapResourcesController.tilemap.GetTile(stonePosition));
        Assert.AreEqual('t', tilemapResourcesController.GetTileOnCords(stonePosition2));
        Assert.AreEqual(2, tilemapResourcesController.treeCount);
        Assert.AreEqual(2, tilemapResourcesController.stoneCount);

        // Act: Clear all tiles
        tilemapResourcesController.ClearAllTiles();

        // Assert: Check that counts are reset and tiles are removed
        Assert.AreEqual(0, tilemapResourcesController.treeCount);
        Assert.AreEqual(0, tilemapResourcesController.stoneCount);
        Assert.IsNull(tilemapResourcesController.tilemap.GetTile(treePosition));
        Assert.IsNull(tilemapResourcesController.tilemap.GetTile(stonePosition));

        // Clean up
        GameObject.DestroyImmediate(tilemapResourcesControllerObject);
    }

    [Test]
    public void TilemapResourcesController_SetTile_ShouldPlaceTreeTile()
    {
        // Arrange
        GameObject tilemapResourcesControllerObject = new GameObject("TilemapResourcesControllerTestObject");
        TilemapResourcesController tilemapResourcesController = tilemapResourcesControllerObject.AddComponent<TilemapResourcesController>();
        tilemapResourcesController.tilemap = tilemapResourcesControllerObject.AddComponent<Tilemap>();
        tilemapResourcesController.treeTile = Resources.Load<Tile>("treeTile");

        // Act: Set a tree tile
        Vector3Int treePosition = new Vector3Int(0, 0, 0);
        tilemapResourcesController.SetTile(treePosition, 't');

        // Assert: Check that the tree tile is placed correctly
        Assert.IsNotNull(tilemapResourcesController.tilemap.GetTile(treePosition));
        Assert.AreEqual('t', tilemapResourcesController.GetTileOnCords(treePosition));
        Assert.AreEqual(1, tilemapResourcesController.treeCount);

        // Clean up
        GameObject.DestroyImmediate(tilemapResourcesControllerObject);
    }

    [Test]
    public void TilemapResourcesController_SetTile_ShouldPlaceStoneTile()
    {
        // Arrange
        GameObject tilemapResourcesControllerObject = new GameObject("TilemapResourcesControllerTestObject");
        TilemapResourcesController tilemapResourcesController = tilemapResourcesControllerObject.AddComponent<TilemapResourcesController>();
        tilemapResourcesController.tilemap = tilemapResourcesControllerObject.AddComponent<Tilemap>();
        tilemapResourcesController.stoneTile = Resources.Load<Tile>("stoneTile");

        // Act: Set a stone tile
        Vector3Int stonePosition = new Vector3Int(10, 10, 0);
        tilemapResourcesController.SetTile(stonePosition, 's');

        // Assert: Check that the stone tile is placed correctly
        Assert.IsNotNull(tilemapResourcesController.tilemap.GetTile(stonePosition));
        Assert.AreEqual('n', tilemapResourcesController.GetTileOnCords(stonePosition));
        Assert.AreEqual(1, tilemapResourcesController.stoneCount);

        // Clean up
        GameObject.DestroyImmediate(tilemapResourcesControllerObject);
    }
}
