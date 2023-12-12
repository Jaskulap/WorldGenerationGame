using UnityEngine;


public interface IInputProvider
{
    Vector2 GetMouseScrollDelta();
}


public class DefaultInputProvider : IInputProvider
{
    public Vector2 GetMouseScrollDelta()
    {
        return Input.mouseScrollDelta;
    }
}


public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 100f;
    public bool allowed = true;
    private Camera mainCamera;
    private Vector3 lastPanPosition;
    private Vector3 origin;

    private IInputProvider inputProvider;

    public void SetInputProvider(IInputProvider inputProvider)
    {
        this.inputProvider = inputProvider;
    }

    public void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            mainCamera = gameObject.AddComponent<Camera>();
        }
        SetInputProvider(new DefaultInputProvider());
    }

    void LateUpdate()
    {
        PanCamera();
        ZoomCamera();
    }

    public void PanCamera()
    {
        // Check if right mouse button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            // Set the origin to the current mouse position in world coordinates
            origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        // Check if right mouse button is held down
        if (Input.GetMouseButton(1))
        {
            // Calculate the offset between the current and previous mouse positions
            Vector3 offset = origin - mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Move the camera position by the offset
            mainCamera.transform.position += offset;
        }
    }

    public void ZoomInCamera()
    {
        if (allowed)
        {
            // Calculate the new zoom level within the specified bounds
            float newZoom = Mathf.Clamp(mainCamera.orthographicSize + zoomSpeed, minZoom, maxZoom);

            // Set the camera size to the new zoom level
            mainCamera.orthographicSize = newZoom;
        }
    }
    public void ZoomOutCamera()
    {
        if (allowed)
        {
            float newZoom = Mathf.Clamp(mainCamera.orthographicSize - zoomSpeed, minZoom, maxZoom);

            mainCamera.orthographicSize = newZoom;
        }
    }

    public void ZoomCamera()
    {
        Vector2 mouseScrollDelta = inputProvider.GetMouseScrollDelta();
        float zoomDelta = -mouseScrollDelta.y;

        if (zoomDelta > 0)
        {
            ZoomInCamera();
        }
        else if (zoomDelta < 0)
        {
            ZoomOutCamera();
        }
    }

}