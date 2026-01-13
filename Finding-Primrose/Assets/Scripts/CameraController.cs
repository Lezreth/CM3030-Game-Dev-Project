using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Movement")]
    public float focusSpeed = 5f;

    [Header("Zoom")]
    public float focusedSize = 4f;   // Smaller = closer
    private float defaultSize;

    private Transform target;
    private Camera cam;
    private bool isFocusing = false;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPos = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPos,
                Time.deltaTime * focusSpeed
            );
        }

        // Smooth zoom
        float targetSize = isFocusing ? focusedSize : defaultSize;
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            Time.deltaTime * focusSpeed
        );
    }

    public void FocusOn(Transform focusTarget)
    {
        target = focusTarget;
        isFocusing = true;
    }

    public void ClearFocus()
    {
        target = null;
        isFocusing = false;
    }
}

