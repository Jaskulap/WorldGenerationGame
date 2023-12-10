using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public bool allowed = true;
    private Camera mainCamera;
    private Vector3 lastPanPosition;
    private Vector3 origin;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        PanCamera();
        ZoomCamera();
    }

    void PanCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 offset = origin - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mainCamera.transform.position += offset;
        }
    }

    void ZoomCamera()
    {   if (allowed)
        {
            float zoomDelta = -Input.mouseScrollDelta.y * zoomSpeed;
            float newZoom = Mathf.Clamp(mainCamera.orthographicSize + zoomDelta, minZoom, maxZoom);
            mainCamera.orthographicSize = newZoom;
        }
    }
}
