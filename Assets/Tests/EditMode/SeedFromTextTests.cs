using NUnit.Framework;

public class SeedFromTextTests
{
    [Test]
    public void SeedFromText_GenerateSeedFromText_ShouldGenerateSeedWithinRange()
    {
        // Arrange
        string text = "sampleText";

        // Act: Generate a seed from the given text
        int generatedSeed = SeedFromText.GenerateSeedFromText(text);

        // Assert: Check if the generated seed is within the expected range
        Assert.GreaterOrEqual(generatedSeed, 0);
        Assert.LessOrEqual(generatedSeed, 1000);
    }

    [Test]
    public void SeedFromText_GenerateSeedFromText_ShouldGenerateUniqueSeeds()
    {
        // Arrange
        string text1 = "sampleText1";
        string text2 = "sampleText2";

        // Act: Generate seeds from two different texts
        int seed1 = SeedFromText.GenerateSeedFromText(text1);
        int seed2 = SeedFromText.GenerateSeedFromText(text2);

        // Assert: Check if the generated seeds are different
        Assert.AreNotEqual(seed1, seed2);
    }

    [Test]
    public void SeedFromText_GenerateSeedFromText_ShouldHandleNullOrEmptyInput()
    {
        // Act: Generate seeds from null and empty string
        int seedFromNull = SeedFromText.GenerateSeedFromText(null);
        int seedFromEmptyString = SeedFromText.GenerateSeedFromText("");

        // Assert: Check if the generated seeds are within the expected range
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

        // Act: Generate seeds twice from the same text
        int seed1 = SeedFromText.GenerateSeedFromText(text);
        int seed2 = SeedFromText.GenerateSeedFromText(text);

        // Assert: Check if the generated seeds are equal (deterministic)
        Assert.AreEqual(seed1, seed2);
    }
}
