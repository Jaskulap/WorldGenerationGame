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
        // Arrange
        List<int> originalList = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        FileHandler.SaveToJSON(originalList, TestFilename);
        List<int> loadedList = FileHandler.ReadListFromJSON<int>(TestFilename);

        // Assert
        CollectionAssert.AreEqual(originalList, loadedList);
    }

    [Test]
    public void SaveAndReadFromJSON_ShouldSaveAndReadObjectCorrectly()
    {
        // Arrange
        MyClass originalObject = new MyClass { Name = "John", Age = 25 };

        // Act
        FileHandler.SaveToJSON(originalObject, TestFilename);
        MyClass loadedObject = FileHandler.ReadFromJSON<MyClass>(TestFilename);

        // Assert
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
