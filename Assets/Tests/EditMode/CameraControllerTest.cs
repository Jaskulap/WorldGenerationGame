using NUnit.Framework;
using UnityEngine;

public class TestInputProvider : IInputProvider
{
    private Vector2 mouseScrollDelta;

    public void SetMouseScrollDelta(Vector2 delta)
    {
        mouseScrollDelta = delta;
    }

    public Vector2 GetMouseScrollDelta()
    {
        return mouseScrollDelta;
    }
}

[TestFixture]
public class CameraControllerTests
{
    private CameraController cameraController;
    private Camera testCamera;

    private TestInputProvider testInputProvider;

    [SetUp]
    public void SetUp()
    {
        GameObject cameraGameObject = new GameObject();
        cameraController = cameraGameObject.AddComponent<CameraController>();

        // Set up the test input provider
        testInputProvider = new TestInputProvider();


        cameraController.Start();

        cameraController.SetInputProvider(testInputProvider);
    }
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(cameraController.gameObject);
    }

    [Test]
    public void ZoomInCamera_IncreasesCameraSize()
    {
        float originalSize = cameraController.GetComponent<Camera>().orthographicSize;

        cameraController.ZoomInCamera();

        Assert.Greater(cameraController.GetComponent<Camera>().orthographicSize, originalSize);
    }

    [Test]
    public void ZoomOutCamera_DecreasesCameraSize()
    {
        float originalSize = cameraController.GetComponent<Camera>().orthographicSize;

        cameraController.ZoomOutCamera();

        Assert.Less(cameraController.GetComponent<Camera>().orthographicSize, originalSize);
    }

    [Test]
    public void ZoomCamera_ScrollUp_ZoomsIn()
    {
        float originalSize = cameraController.GetComponent<Camera>().orthographicSize;

        // Simulate a scroll up by setting a positive zoomDelta
        testInputProvider.SetMouseScrollDelta(new Vector2(0, 1));

        cameraController.ZoomCamera();

        float newSize = cameraController.GetComponent<Camera>().orthographicSize;
        Assert.Less(newSize, originalSize);
    }

    [Test]
    public void ZoomCamera_ScrollDown_ZoomsOut()
    {
        float originalSize = cameraController.GetComponent<Camera>().orthographicSize;

        // Simulate a scroll down by setting a negative zoomDelta
        testInputProvider.SetMouseScrollDelta(new Vector2(0, -1));

        cameraController.ZoomCamera();

        float newSize = cameraController.GetComponent<Camera>().orthographicSize;
        Assert.Greater(newSize, originalSize);
    }

}