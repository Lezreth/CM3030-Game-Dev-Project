//Reframes the scene for Choice UI interaction
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    
    public CameraFollow cameraFollow; 

    [Header("Movement")]
    [SerializeField] float focusSpeed = 5f;
    [SerializeField] float arriveThreshold = 0.05f;

    private Transform target;
    private bool isFocusing = false;

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
        isFocusing = true;
        if (cameraFollow != null) cameraFollow.enabled = false;
    }

    public void ClearFocus()
    {
        target = null;
        isFocusing = false;
        if (cameraFollow != null) cameraFollow.enabled = true;
    }

    public bool HasArrived()
    {
        if (!isFocusing || target == null)
            return true;

        Vector3 desiredPos = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        return Vector3.Distance(transform.position, desiredPos) < arriveThreshold;
    }
}