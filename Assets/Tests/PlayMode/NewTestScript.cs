using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        //   GameObject testObject = new GameObject("TestObject");

        // Add the WorldGeneration script to the GameObject
        //   WorldGeneration worldGeneration = testObject.AddComponent<WorldGeneration>();

        // Call the method on the script
        //   worldGeneration.GenerateEmptyWorld();

        // Assert that the script is not null
        //   Assert.IsNotNull(worldGeneration);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {

        yield return null;
    }
}
