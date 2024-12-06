using UnityEngine;

namespace CodeBase.CameraMovement
{
    [RequireComponent(typeof(Camera))]
    public class RTSCamera : MonoBehaviour
    {
       [Header("Panning Settings")]
    public float panSpeed = 20f;        // Speed of camera movement
    public float panBorderThickness = 10f; // Border thickness for screen-edge panning

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.5f;     // Speed of camera zoom
    public float minZoom = 10f;        // Minimum camera height
    public float maxZoom = 50f;        // Maximum camera height

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;  // Speed of camera rotation
    public bool allowRotation = true;  // Allow camera rotation

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        transform.position = new Vector3(500, transform.position.y, 500);
    }

    private void Update()
    {
        HandlePanning();
        HandleZooming();
        if (allowRotation)
        {
            HandleRotation();
        }
    }

    private void HandlePanning()
    {
        Vector3 move = Vector3.zero;

        if (Input.touchCount == 1) // One-finger drag for panning
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                move = new Vector3(-delta.x, 0, -delta.y) * panSpeed * Time.deltaTime;
            }
        }

        transform.Translate(move, Space.World);
    }

    private void HandleZooming()
    {
        if (Input.touchCount == 2) // Two-finger pinch for zooming
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            float targetZoom = cam.orthographicSize - difference * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }

    private void HandleRotation()
    {
        if (Input.touchCount == 2) // Two-finger swipe for rotation
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
            {
                Vector2 touch0Delta = touch0.deltaPosition;
                Vector2 touch1Delta = touch1.deltaPosition;

                float rotationDelta = (touch0Delta.x + touch1Delta.x) / 2;
                transform.Rotate(Vector3.up, -rotationDelta * rotationSpeed * Time.deltaTime, Space.World);
            }
        }
    }
    }
}