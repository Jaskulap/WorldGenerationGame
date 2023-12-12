using NUnit.Framework;

[TestFixture]
public class TilemapBiomesControllerTests
{
    private TilemapBiomesController tilemapBiomesController;

    [SetUp]
    public void SetUp()
    {
        // Arrange: Set up TilemapBiomesController and initialize with test tiles
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
        // Act: Categorize islands in the tilemap
        tilemapBiomesController.CategorizeIslands();

        // Assert: Check if the islands are categorized correctly
        Assert.IsTrue(tilemapBiomesController.categorized);
        Assert.AreEqual(tilemapBiomesController.tilemapIslandNumber[0, 0], 0);
        Assert.AreEqual(tilemapBiomesController.tilemapIslandNumber[1, 1], 1);
        Assert.AreEqual(tilemapBiomesController.tilemapIslandNumber[3, 3], 2);
    }

    [Test]
    public void FloodFill_ShouldFillIslandCorrectly()
    {
        // Act: Perform flood-fill starting from coordinates (1, 1) with island number 1
        tilemapBiomesController.FloodFill(1, 1, 1);

        // Assert: Check if the flood-filled island has the correct values in tilemapIslandNumber array
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[0, 2]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[0, 3]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[1, 1]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[1, 2]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[2, 0]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[2, 1]);
        Assert.AreEqual(1, tilemapBiomesController.tilemapIslandNumber[3, 0]);

        // Assert: Check if other tiles outside the flood-fill area have island number 0
        Assert.AreEqual(0, tilemapBiomesController.tilemapIslandNumber[0, 0]);
        Assert.AreEqual(0, tilemapBiomesController.tilemapIslandNumber[3, 3]);
    }
}
