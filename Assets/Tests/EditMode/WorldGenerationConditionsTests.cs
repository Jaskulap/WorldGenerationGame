using NUnit.Framework;
using UnityEngine;

public class WorldGenerationConditionsTests
{
    private WorldGenerationConditions worldGenerationConditions;

    [SetUp]
    public void SetUp()
    {
        // Initialize or create necessary dependencies
        GameObject go = new GameObject();
        worldGenerationConditions = go.AddComponent<WorldGenerationConditions>();
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up or destroy dependencies
        Object.DestroyImmediate(worldGenerationConditions.gameObject);
    }

    [Test]
    public void IsGrassBiome_ShouldReturnTrueInRange()
    {

        float biome = 0.3f;


        bool result = worldGenerationConditions.IsGrassBiome(biome);


        Assert.IsTrue(result);
    }

    [Test]
    public void IsGrassBiome_ShouldReturnFalseOutsideRange()
    {

        float[] outsideRangeBiomes = { 0.1f, 0.2f, 0.51f, 0.6f };


        foreach (float biome in outsideRangeBiomes)
        {
            Assert.IsFalse(worldGenerationConditions.IsGrassBiome(biome));
        }
    }

    [Test]
    public void IsSwampBiome_ShouldReturnTrueInRange()
    {

        float biome = 0.35f;
        float swampNoise = 0.7f;


        bool result = worldGenerationConditions.IsSwampBiome(biome, swampNoise);

        Assert.IsTrue(result);
    }

    [Test]
    public void IsSwampBiome_ShouldReturnFalseOutsideRange()
    {

        float[] outsideRangeBiomes = { 0.1f, 0.2f, 0.45f, 0.6f };
        float[] outsideRangeSwampNoises = { 0.5f, 0.61f, 0.2f, 0.69f };


        for (int i = 0; i < outsideRangeBiomes.Length; i++)
        {
            Assert.IsFalse(worldGenerationConditions.IsSwampBiome(outsideRangeBiomes[i], outsideRangeSwampNoises[i]));
        }
    }
}
