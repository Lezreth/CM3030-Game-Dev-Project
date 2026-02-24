////Reframes the scene for Choice UI interaction
//using UnityEngine;

//public class CameraController : MonoBehaviour
//{
//    public static CameraController Instance;

//    [Header("Movement")]
//    [SerializeField] float focusSpeed = 5f;
//    [SerializeField] float arriveThreshold = 0.05f;

//    private Transform target;
//    private bool isFocusing = false;

//    void Awake()
//    {
//        Instance = this;
//    }

//    void LateUpdate()
//    {
//        if (target == null) return;

//        Vector3 desiredPos = new Vector3(
//            target.position.x,
//            target.position.y,
//            transform.position.z
//        );

//        transform.position = Vector3.Lerp(
//            transform.position,
//            desiredPos,
//            Time.deltaTime * focusSpeed
//        );
//    }

//    public void FocusOn(Transform focusTarget)
//    {
//        target = focusTarget;
//        isFocusing = true;
//    }

//    public void ClearFocus()
//    {
//        target = null;
//        isFocusing = false;
//    }

//    public bool HasArrived()
//    {
//        if (!isFocusing || target == null)
//            return true;

//        Vector3 desiredPos = new Vector3(
//            target.position.x,
//            target.position.y,
//            transform.position.z
//        );

//        return Vector3.Distance(transform.position, desiredPos) < arriveThreshold;
//    }
//}

// This version was made for Orthographic camera view. I will keep for now it in case we switch back to it. 
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Movement")]
    [SerializeField] float focusSpeed = 5f;
    [SerializeField] float arriveThreshold = 0.05f;

    [Header("Zoom")]
    [SerializeField] float focusedSize = 4f;
    //private float defaultSize;

    private Transform target;
    private Camera cam;
    private bool isFocusing = false;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        //defaultSize = cam.orthographicSize;
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

        //float targetSize = isFocusing ? focusedSize : defaultSize;
        //cam.orthographicSize = Mathf.Lerp(
        //    cam.orthographicSize,
        //    targetSize,
        //    Time.deltaTime * focusSpeed
        //);
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

    public bool HasArrived()
    {
        if (!isFocusing || target == null)
            return true;

        Vector3 desiredPos = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        bool positionArrived =
            Vector3.Distance(transform.position, desiredPos) < arriveThreshold;

        //bool zoomArrived =
        //    Mathf.Abs(cam.orthographicSize - focusedSize) < arriveThreshold;

        //return positionArrived && zoomArrived;
        return positionArrived;
    }
}
