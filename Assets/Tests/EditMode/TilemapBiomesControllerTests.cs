using NUnit.Framework;

[TestFixture]
public class TilemapBiomesControllerTests
{
    private TilemapBiomesController tilemapBiomesController;

    [SetUp]
    public void SetUp()
    {
        tilemapBiomesController = new TilemapBiomesController();
        char[,] testTiles = {
            { 'w', 'w', 'g', 'g' },
            { 'w', 'g', 'g', 'w' },
            { 'g', 'g', 'w', 'w' },
            { 'g', 'w', 'w', 'g' }
        };
        tilemapBiomesController.tilemapChars = testTiles;
        tilemapBiomesController.Initialize();
    }

    [Test]
    public void CategorizeIslands_ShouldCategorizeTilesCorrectly()
    {
        tilemapBiomesController.CategorizeIslands();


        Assert.IsTrue(tilemapBiomesController.categorized);
        Assert.AreEqual(tilemapBiomesController.tilemapIslandNumber[0, 0], 0);
        Assert.AreEqual(tilemapBiomesController.tilemapIslandNumber[1, 1], 1);
        Assert.AreEqual(tilemapBiomesController.tilemapIslandNumber[3, 3], 2);
    }
    [Test]
    public void FloodFill_ShouldFillIslandCorrectly()
    {




        tilemapBiomesController.FloodFill(1, 1, 1);

        // Assert: Check if the flood-filled island has the correct values in tilemapIslandNumber array
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[0, 2]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[0, 3]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[1, 1]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[1, 2]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[2, 0]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[2, 1]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[3, 0]);

        // Additional assertions based on your requirements
        // For example, you can check that other positions are not part of the island
        Assert.AreEqual(0, tilemapBiomesController.tilemapIslandNumber[0, 0]);
        Assert.AreEqual(0, tilemapBiomesController.tilemapIslandNumber[3, 3]);
    }

}
