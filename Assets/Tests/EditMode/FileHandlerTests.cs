using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FileHandlerTests
{
    private const string TestFilename = "testfile.json";

    [TearDown]
    public void Teardown()
    {
        // Clean up: Delete the test file after each test
        string path = Application.persistentDataPath + "/" + TestFilename;
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }

    [Test]
    public void SaveAndReadListFromJSON_ShouldSaveAndReadListCorrectly()
    {
        // Arrange: Create a sample list
        List<int> originalList = new List<int> { 1, 2, 3, 4, 5 };

        // Act: Save and load the list from JSON
        FileHandler.SaveToJSON(originalList, TestFilename);
        List<int> loadedList = FileHandler.ReadListFromJSON<int>(TestFilename);

        // Assert: Check if the loaded list matches the original list
        CollectionAssert.AreEqual(originalList, loadedList);
    }

    [Test]
    public void SaveAndReadFromJSON_ShouldSaveAndReadObjectCorrectly()
    {
        // Arrange: Create a sample object
        MyClass originalObject = new MyClass { Name = "John", Age = 25 };

        // Act: Save and load the object from JSON
        FileHandler.SaveToJSON(originalObject, TestFilename);
        MyClass loadedObject = FileHandler.ReadFromJSON<MyClass>(TestFilename);

        // Assert: Check if the loaded object's properties match the original object
        Assert.AreEqual(originalObject.Name, loadedObject.Name);
        Assert.AreEqual(originalObject.Age, loadedObject.Age);
    }
}

[System.Serializable]
public class MyClass
{
    public string Name;
    public int Age;
}
