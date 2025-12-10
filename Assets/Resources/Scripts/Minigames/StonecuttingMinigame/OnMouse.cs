using UnityEngine;

public class OnMouse : MonoBehaviour
{
    [Header("Plane Settings")]
    [Tooltip("Distance from the camera to place the plane")]
    public float planeDistance = 3f;

    [Tooltip("Rotation speed when using the mouse wheel")]
    public float rotationSpeed = 50f;


    public Camera mainCam;

    void Start()
    {
        if (mainCam == null)
        {
            Debug.LogError("No main camera found! Please tag your camera as 'MainCamera'.");
        }
    }

    void Update()
    { 
        if (mainCam == null || mainCam.enabled == false) return;
        float rotationSpeedDupe = rotationSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            rotationSpeedDupe *= 5;
        }

        // --- 1. Move object to follow mouse cursor ---
        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPos = mouseRay.origin + mouseRay.direction * planeDistance;
        transform.position = targetPos;

        // --- 2. Rotate with mouse wheel ---
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            transform.Rotate(new Vector3(1,0,0), scroll * rotationSpeedDupe, Space.Self);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.005f);
    }
}
