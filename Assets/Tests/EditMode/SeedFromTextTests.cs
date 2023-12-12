using NUnit.Framework;

public class SeedFromTextTests
{
    [Test]
    public void SeedFromText_GenerateSeedFromText_ShouldGenerateSeedWithinRange()
    {
        // Arrange
        string text = "sampleText";

        // Act
        int generatedSeed = SeedFromText.GenerateSeedFromText(text);

        // Assert
        Assert.GreaterOrEqual(generatedSeed, 0);
        Assert.LessOrEqual(generatedSeed, 1000);
    }

    [Test]
    public void SeedFromText_GenerateSeedFromText_ShouldGenerateUniqueSeeds()
    {
        // Arrange
        string text1 = "sampleText1";
        string text2 = "sampleText2";

        // Act
        int seed1 = SeedFromText.GenerateSeedFromText(text1);
        int seed2 = SeedFromText.GenerateSeedFromText(text2);

        // Assert
        Assert.AreNotEqual(seed1, seed2);
    }

    [Test]
    public void SeedFromText_GenerateSeedFromText_ShouldHandleNullOrEmptyInput()
    {
        // Arrange

        // Act
        int seedFromNull = SeedFromText.GenerateSeedFromText(null);
        int seedFromEmptyString = SeedFromText.GenerateSeedFromText("");

        // Assert
        Assert.GreaterOrEqual(seedFromNull, 0);
        Assert.LessOrEqual(seedFromNull, 1000);
        Assert.GreaterOrEqual(seedFromEmptyString, 0);
        Assert.LessOrEqual(seedFromEmptyString, 1000);
    }

    [Test]
    public void SeedFromText_GenerateSeedFromText_ShouldProduceDeterministicResults()
    {
        // Arrange
        string text = "sampleText";

        // Act
        int seed1 = SeedFromText.GenerateSeedFromText(text);
        int seed2 = SeedFromText.GenerateSeedFromText(text);

        // Assert
        Assert.AreEqual(seed1, seed2);
    }
}
