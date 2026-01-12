using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public float focusSpeed = 5f;
    private Transform target;

    void Awake()
    {
        Instance = this;
    }

    void LateUpdate()
    {
        if (target == null) return;

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

    public void FocusOn(Transform focusTarget)
    {
        target = focusTarget;
    }

    public void ClearFocus()
    {
        target = null;
    }
}

