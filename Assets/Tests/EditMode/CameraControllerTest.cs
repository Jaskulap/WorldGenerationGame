using NUnit.Framework;
using UnityEngine;

// Test input provider implementation for simulating input in tests
public class TestInputProvider : IInputProvider
{
    private Vector2 mouseScrollDelta;

    // Set the simulated mouse scroll delta for testing
    public void SetMouseScrollDelta(Vector2 delta)
    {
        mouseScrollDelta = delta;
    }

    // Retrieve the simulated mouse scroll delta for testing
    public Vector2 GetMouseScrollDelta()
    {
        return mouseScrollDelta;
    }
}

// NUnit test fixture for CameraController class
[TestFixture]
public class CameraControllerTests
{
    private CameraController cameraController;
    private Camera testCamera;
    private TestInputProvider testInputProvider;

    // Setup method to initialize objects before each test
    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject and add CameraController component
        GameObject cameraGameObject = new GameObject();
        cameraController = cameraGameObject.AddComponent<CameraController>();

        // Set up the test input provider
        testInputProvider = new TestInputProvider();

        // Initialize CameraController and set the test input provider
        cameraController.Start();
        cameraController.SetInputProvider(testInputProvider);
    }

    // Teardown method to clean up objects after each test
    [TearDown]
    public void TearDown()
    {
        // Destroy the CameraController GameObject immediately after each test
        Object.DestroyImmediate(cameraController.gameObject);
    }

    // Test case: Zooming in should increase the camera size
    [Test]
    public void ZoomInCamera_IncreasesCameraSize()
    {
        // Arrange
        float originalSize = cameraController.GetComponent<Camera>().orthographicSize;

        // Act: Call the ZoomInCamera method
        cameraController.ZoomInCamera();

        // Assert: Check if the camera size has increased
        Assert.Greater(cameraController.GetComponent<Camera>().orthographicSize, originalSize);
    }

    // Test case: Zooming out should decrease the camera size
    [Test]
    public void ZoomOutCamera_DecreasesCameraSize()
    {
        // Arrange
        float originalSize = cameraController.GetComponent<Camera>().orthographicSize;

        // Act: Call the ZoomOutCamera method
        cameraController.ZoomOutCamera();

        // Assert: Check if the camera size has decreased
        Assert.Less(cameraController.GetComponent<Camera>().orthographicSize, originalSize);
    }

    // Test case: Zooming in with simulated scroll up should decrease camera size
    [Test]
    public void ZoomCamera_ScrollUp_ZoomsIn()
    {
        // Arrange
        float originalSize = cameraController.GetComponent<Camera>().orthographicSize;

        // Act: Simulate a scroll up and call the ZoomCamera method
        testInputProvider.SetMouseScrollDelta(new Vector2(0, 1));
        cameraController.ZoomCamera();

        // Assert: Check if the camera size has decreased
        float newSize = cameraController.GetComponent<Camera>().orthographicSize;
        Assert.Less(newSize, originalSize);
    }

    // Test case: Zooming out with simulated scroll down should increase camera size
    [Test]
    public void ZoomCamera_ScrollDown_ZoomsOut()
    {
        // Arrange
        float originalSize = cameraController.GetComponent<Camera>().orthographicSize;

        // Act: Simulate a scroll down and call the ZoomCamera method
        testInputProvider.SetMouseScrollDelta(new Vector2(0, -1));
        cameraController.ZoomCamera();

        // Assert: Check if the camera size has increased
        float newSize = cameraController.GetComponent<Camera>().orthographicSize;
        Assert.Greater(newSize, originalSize);
    }
}
