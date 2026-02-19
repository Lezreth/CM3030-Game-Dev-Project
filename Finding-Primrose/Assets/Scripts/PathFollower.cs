using UnityEngine;
using UnityEngine.InputSystem;

public class PathFollower : MonoBehaviour
{
    [Header("Paths")]
    public PathData frontPath;
    public PathData middlePath;
    public PathData backPath;
    public PathData backBackPath;
    public PathData currentPath;

    [Header("Animation")]  
    public Animator animator;

    [Header("Visuals")]
    public GameObject clickMarkerPrefab;
    public Camera mainCamera;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;
    public float stoppingDistance = 0.1f;

    [Header("Inner Hallway Boundaries")]
    [Tooltip("Left inner hallway - inner edge (closest to center)")]
    public float leftInnerHallwayInner = -2.1f;
    [Tooltip("Left inner hallway - outer edge (further from center)")]
    public float leftInnerHallwayOuter = -3.3f;
    [Tooltip("Right inner hallway - inner edge (closest to center)")]
    public float rightInnerHallwayInner = 2.2f;
    [Tooltip("Right inner hallway - outer edge (further from center)")]
    public float rightInnerHallwayOuter = 3.4f;

    [Header("Outer Hallway Boundaries")]
    [Tooltip("Left outer hallway - inner edge")]
    public float leftOuterHallwayInner = -8f;
    [Tooltip("Left outer hallway - outer edge")]
    public float leftOuterHallwayOuter = -10.5f;
    [Tooltip("Right outer hallway - inner edge")]
    public float rightOuterHallwayInner = 7.7f;
    [Tooltip("Right outer hallway - outer edge")]
    public float rightOuterHallwayOuter = 9.7f;

    [Header("Marker")]
    public float markerLifetime = 0.5f;
    public float markerYOffset = 0.1f;

    [Header("Transition Settings")]
    public float depthThreshold = 1.5f;
    public float transitionDuration = 0.5f;
    public bool requireHallwayForTransitions = true;

    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool showBoundaries = true;

    // State
    private Vector3 targetPosition;
    private bool isMoving;
    private bool isTransitioning;
    private bool movementLocked;

    // Transition state
    private float transitionTimer;
    private Vector3 transitionStartPos;
    private Vector3 transitionEndPos;
    private float transitionHallwayX;

    private Vector3 lastClickPosition;
    private string debugMessage = "";

    // =====================
    // UNITY LIFECYCLE
    // =====================

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (currentPath == null)
            currentPath = frontPath;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        Debug.Log($"[PathFollower] Animator found: {(animator != null ? "YES" : "NO")}");
        if (animator != null)
        {
            Debug.Log($"[PathFollower] Animator name: {animator.name}");
            Debug.Log($"[PathFollower] Has 'Walk' param: {HasParameter(animator, "Walk")}");
        }
    }

    void Update()
    {
        CheckWaypointExits();

        if (movementLocked) return;

        if (isTransitioning)
        {
            HandleTransition();
            return;
        }

        HandleInput();

        if (isMoving)
            MoveToTarget();

        CheckWaypointTriggers();
    }

    // =====================
    // INPUT & MOVEMENT
    // =====================

    void HandleInput()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            debugMessage = "no collider hit";
            return;
        }

        lastClickPosition = hit.point;

        float zDiff = hit.point.z - transform.position.z;
        bool isTransitionClick = Mathf.Abs(zDiff) >= depthThreshold;

        Vector3 processedClick = hit.point;

        if (isTransitionClick && requireHallwayForTransitions)
        {
            processedClick = ClampToHallway(hit.point);
            Debug.Log($"Transition click: Original X={hit.point.x:F2} → Clamped X={processedClick.x:F2}");
        }

        if (TryStartPathTransition(processedClick)) return;

        MoveOnCurrentPath(processedClick);
    }

    void MoveOnCurrentPath(Vector3 clickPosition)
    {
        if (currentPath == null)
        {
            debugMessage = "Current path is NULL!";
            return;
        }

        targetPosition = currentPath.GetClosestPointOnPath(clickPosition);
        isMoving = true;
        ShowClickMarker(targetPosition);
        debugMessage = "Moving on current path";
    }

    void MoveToTarget()
    {
        Vector3 currentPos = transform.position;
        Vector3 moveDirection = targetPosition - currentPos;
        moveDirection.y = 0;

        float distanceToTarget = moveDirection.magnitude;

        if (distanceToTarget > stoppingDistance)
        {
            if (animator != null)
                animator.SetBool("Walk", true);

            transform.position = Vector3.MoveTowards(currentPos, targetPosition, moveSpeed * Time.deltaTime);

            if (moveDirection.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (animator != null)
                animator.SetBool("Walk", false);

            isMoving = false;
            transform.position = targetPosition;
        }
    }

    public void LockMovement()
    {
        movementLocked = true;
        isMoving = false;
        isTransitioning = false;

        if (animator != null)
            animator.SetBool("Walk", false);
    }

    public void UnlockMovement()
    {
        movementLocked = false;
    }

    // =====================
    // PATH TRANSITIONS
    // =====================

    bool TryStartPathTransition(Vector3 clickPosition)
    {
        if (!ShouldTransition(clickPosition)) return false;

        PathData targetPath = GetTargetPath(clickPosition);

        if (targetPath == null)
        {
            debugMessage = "Target path is NULL";
            return false;
        }

        if (targetPath == currentPath)
        {
            debugMessage = "Already on target path";
            return false;
        }

        TransitionToPath(targetPath, clickPosition.x);
        return true;
    }

    bool ShouldTransition(Vector3 clickPosition)
    {
        float zDifference = clickPosition.z - transform.position.z;

        if (Mathf.Abs(zDifference) < depthThreshold)
        {
            Debug.Log($"[ShouldTransition] FAILED - Z diff {Mathf.Abs(zDifference):F2} < threshold {depthThreshold}");
            return false;
        }

        // If hallway check is disabled, allow any transition (e.g. Food Trucks scene)
        if (!requireHallwayForTransitions)
        {
            Debug.Log("[ShouldTransition] Hallway check disabled - allowing transition");
            return true;
        }

        bool inHallway = IsInHallway(clickPosition.x);
        Debug.Log($"[ShouldTransition] Click X: {clickPosition.x:F2}, In hallway: {inHallway}");
        return inHallway;
    }

    bool IsInHallway(float xPosition)
    {
        bool inLeftInner  = xPosition >= leftInnerHallwayOuter && xPosition <= leftInnerHallwayInner;
        bool inRightInner = xPosition >= rightInnerHallwayInner && xPosition <= rightInnerHallwayOuter;
        bool inLeftOuter  = xPosition >= leftOuterHallwayOuter && xPosition <= leftOuterHallwayInner;
        bool inRightOuter = xPosition >= rightOuterHallwayInner && xPosition <= rightOuterHallwayOuter;

        return inLeftInner || inRightInner || inLeftOuter || inRightOuter;
    }

    PathData GetTargetPath(Vector3 clickPosition)
    {
        // Find whichever path Z is closest to the click - dog moves to nearest path
        PathData closest = null;
        float closestDist = float.MaxValue;

        PathData[] allPaths = { frontPath, middlePath, backPath, backBackPath };

        foreach (PathData path in allPaths)
        {
            if (path == null || path == currentPath) continue;

            Vector3 pathPoint = path.GetClosestPointOnPath(clickPosition);
            float dist = Mathf.Abs(pathPoint.z - clickPosition.z);

            Debug.Log($"[GetTargetPath] {GetPathName(path)} | Z dist to click: {dist:F2}");

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = path;
            }
        }

        Debug.Log($"[GetTargetPath] Closest path: {GetPathName(closest)}");
        return closest;
    }

    void TransitionToPath(PathData newPath, float clickX)
    {
        if (newPath == null || isTransitioning) return;

        transitionHallwayX = clickX;

        Vector3 newPathPoint = newPath.GetClosestPointOnPath(transform.position);
        newPathPoint.x = transitionHallwayX;

        isTransitioning = true;
        isMoving = false;
        transitionTimer = 0f;
        transitionStartPos = transform.position;
        transitionEndPos = newPathPoint;
        currentPath = newPath;

        Debug.Log($"Started transition to {GetPathName(newPath)} at X={clickX:F2}");
    }

    void HandleTransition()
    {
        if (animator != null)
            animator.SetBool("Walk", true);

        transitionTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(transitionTimer / transitionDuration);

        Vector3 newPos = Vector3.Lerp(transitionStartPos, transitionEndPos, progress);
        newPos.x = transitionHallwayX;
        transform.position = newPos;

        Vector3 direction = transitionEndPos - transitionStartPos;
        direction.y = 0;
        direction.x = 0;

        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (progress >= 1f)
        {
            if (animator != null)
                animator.SetBool("Walk", false);

            isTransitioning = false;
            transform.position = transitionEndPos;
            Debug.Log($"Transition complete. Now on: {GetPathName(currentPath)}");
        }
    }

    // =====================
    // WAYPOINTS
    // =====================

    void CheckWaypointExits()
    {
        if (currentPath?.waypoints == null) return;

        foreach (Transform wp in currentPath.waypoints)
        {
            if (wp == null) continue;
            wp.GetComponent<ChoiceWaypoint>()?.CheckExit(transform.position);
            wp.GetComponent<TransitionWaypoint>()?.CheckExit(transform.position);
        }
    }

    void CheckWaypointTriggers()
    {
        if (movementLocked || currentPath?.waypoints == null) return;

        foreach (Transform wp in currentPath.waypoints)
        {
            if (wp == null) continue;

            ChoiceWaypoint choice = wp.GetComponent<ChoiceWaypoint>();
            if (choice != null && choice.CanTrigger(transform.position))
            {
                choice.Trigger();
                return;
            }

            TransitionWaypoint transition = wp.GetComponent<TransitionWaypoint>();
            if (transition != null && transition.CanTrigger(transform.position))
            {
                transition.Trigger();
                return;
            }
        }
    }

    // =====================
    // HELPERS
    // =====================

    Vector3 ClampToHallway(Vector3 clickPosition)
    {
        Vector3 clamped = clickPosition;
        float x = clickPosition.x;

        if (x < 0)
        {
            if (x <= leftOuterHallwayInner)
            {
                clamped.x = Mathf.Clamp(x, leftOuterHallwayOuter, leftOuterHallwayInner);
                debugMessage = "In left outer hallway";
            }
            else
            {
                clamped.x = Mathf.Clamp(x, leftInnerHallwayOuter, leftInnerHallwayInner);
                debugMessage = "In left inner hallway";
            }
        }
        else if (x > 0)
        {
            if (x >= rightOuterHallwayInner)
            {
                clamped.x = Mathf.Clamp(x, rightOuterHallwayInner, rightOuterHallwayOuter);
                debugMessage = "In right outer hallway";
            }
            else
            {
                clamped.x = Mathf.Clamp(x, rightInnerHallwayInner, rightInnerHallwayOuter);
                debugMessage = "In right inner hallway";
            }
        }

        return clamped;
    }

    void ShowClickMarker(Vector3 position)
    {
        if (clickMarkerPrefab == null) return;
        Vector3 markerPos = position + Vector3.up * markerYOffset;
        GameObject marker = Instantiate(clickMarkerPrefab, markerPos, Quaternion.identity);
        Destroy(marker, markerLifetime);
    }

    string GetPathName(PathData path)
    {
        if (path == frontPath)   return "FRONT";
        if (path == middlePath)  return "MIDDLE";
        if (path == backPath)    return "BACK";
        if (path == backBackPath) return "BACK-BACK";
        return "UNKNOWN";
    }

    bool HasParameter(Animator anim, string paramName)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
            if (param.name == paramName) return true;
        return false;
    }
}