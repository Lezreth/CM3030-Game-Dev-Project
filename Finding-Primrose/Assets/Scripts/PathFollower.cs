using UnityEngine;
using UnityEngine.InputSystem;

public class PathFollower : MonoBehaviour
{
    [Header("Paths")]
    public PathData frontPath;
    public PathData middlePath;
    public PathData backPath;
    public PathData currentPath;

    [Header("Visuals")]
    public GameObject clickMarkerPrefab;
    public Camera mainCamera;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 0.1f;

    [Header("Marker")]
    public float markerLifetime = 0.5f;
    public float markerYOffset = 0.1f;

    [Header("Transition Settings")]
    public float depthThreshold = 1.5f;
    public float transitionDuration = 0.5f;

    [Header("Debug")]
    public bool showDebugInfo = true;

    // Constants for transition gaps
    private const float LEFT_GAP_MIN = -3.75f;
    private const float LEFT_GAP_MAX = -1.75f;
    private const float RIGHT_GAP_MIN = 1.75f;
    private const float RIGHT_GAP_MAX = 3.75f;
    private const float MIN_ROTATION_THRESHOLD = 0.1f;

    // State
    private Vector3 targetPosition;
    private bool isMoving;
    private bool isTransitioning;
    private bool movementLocked;

    // Transition state
    private float transitionTimer;
    private Vector3 transitionStartPos;
    private Vector3 transitionEndPos;

    // Debug info
    private Vector3 lastClickPosition;
    private string debugMessage = "";

    // ============================================
    // UNITY LIFECYCLE
    // ============================================

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (currentPath == null)
            currentPath = frontPath;
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

    // ============================================
    // INPUT HANDLING
    // ============================================

    void HandleInput()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            debugMessage = "Click missed - no collider hit";
            return;
        }

        lastClickPosition = hit.point;

        // Try path transition first
        if (TryStartPathTransition(hit.point)) return;

        // Otherwise, move on current path
        MoveOnCurrentPath(hit.point);
    }

    bool TryStartPathTransition(Vector3 clickPosition)
    {
        if (!ShouldTransition(clickPosition))
        {
            debugMessage = "Not a valid transition area";
            return false;
        }

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

        TransitionToPath(targetPath);
        return true;
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

    // ============================================
    // MOVEMENT
    // ============================================

    void MoveToTarget()
    {
        // Move horizontally
        Vector3 moveDirection = targetPosition - transform.position;
        moveDirection.y = 0;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Rotate toward target
        if (moveDirection.magnitude > MIN_ROTATION_THRESHOLD)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Check if reached destination
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            isMoving = false;
        }
    }

    public void LockMovement()
    {
        movementLocked = true;
        isMoving = false;
        isTransitioning = false;
    }

    public void UnlockMovement()
    {
        movementLocked = false;
    }

    // ============================================
    // PATH TRANSITIONS
    // ============================================

    bool ShouldTransition(Vector3 clickPosition)
    {
        float zDifference = clickPosition.z - transform.position.z;

        // Must click far enough forward/back
        if (Mathf.Abs(zDifference) < depthThreshold)
        {
            Debug.Log($"Z diff too small: {Mathf.Abs(zDifference):F2} < {depthThreshold}");
            return false;
        }

        // Must click in a transition gap
        bool inGap = IsInTransitionGap(clickPosition.x);
        if (!inGap)
        {
            Debug.Log($"Not in transition gap. X: {clickPosition.x:F2}");
        }
        return inGap;
    }

    bool IsInTransitionGap(float xPosition)
    {
        bool inLeftGap = xPosition >= LEFT_GAP_MIN && xPosition <= LEFT_GAP_MAX;
        bool inRightGap = xPosition >= RIGHT_GAP_MIN && xPosition <= RIGHT_GAP_MAX;
        return inLeftGap || inRightGap;
    }

    PathData GetTargetPath(Vector3 clickPosition)
    {
        float zDifference = clickPosition.z - transform.position.z;
        bool clickingBackward = zDifference > 0;

        string currentPathName = GetPathName(currentPath);
        Debug.Log($"Current: {currentPathName}, Z diff: {zDifference:F2}, Clicking: {(clickingBackward ? "BACKWARD" : "FORWARD")}");

        // Front → Middle (backward only)
        if (currentPath == frontPath)
        {
            if (clickingBackward)
            {
                Debug.Log("Transitioning: Front → Middle");
                debugMessage = "Front → Middle";
                return middlePath;
            }
            Debug.Log("Can't go forward from Front path");
            return null;
        }

        // Middle → Front or Back
        if (currentPath == middlePath)
        {
            if (clickingBackward)
            {
                Debug.Log("Transitioning: Middle → Back");
                debugMessage = "Middle → Back";
                return backPath;
            }
            else
            {
                Debug.Log("Transitioning: Middle → Front");
                debugMessage = "Middle → Front";
                return frontPath;
            }
        }

        // Back → Middle (forward only)
        if (currentPath == backPath)
        {
            if (!clickingBackward)
            {
                Debug.Log("Transitioning: Back → Middle");
                debugMessage = "Back → Middle";
                return middlePath;
            }
            Debug.Log("Can't go backward from Back path");
            return null;
        }

        return null;
    }

    void TransitionToPath(PathData newPath)
    {
        if (newPath == null || isTransitioning) return;

        Vector3 newPathPoint = newPath.GetClosestPointOnPath(transform.position);

        isTransitioning = true;
        isMoving = false;
        transitionTimer = 0f;
        transitionStartPos = transform.position;
        transitionEndPos = newPathPoint;
        currentPath = newPath;

        Debug.Log($"Started transition to {GetPathName(newPath)}");
    }

    void HandleTransition()
    {
        transitionTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(transitionTimer / transitionDuration);

        transform.position = Vector3.Lerp(transitionStartPos, transitionEndPos, progress);

        if (progress >= 1f)
        {
            isTransitioning = false;
            transform.position = transitionEndPos;
            Debug.Log($"Transition complete. Now on: {GetPathName(currentPath)}");
        }
    }

    // ============================================
    // WAYPOINT CHECKS
    // ============================================

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

            // Check choice waypoints
            ChoiceWaypoint choice = wp.GetComponent<ChoiceWaypoint>();
            if (choice != null && choice.CanTrigger(transform.position))
            {
                choice.Trigger();
                return;
            }

            // Check transition waypoints
            TransitionWaypoint transition = wp.GetComponent<TransitionWaypoint>();
            if (transition != null && transition.CanTrigger(transform.position))
            {
                transition.Trigger();
                return;
            }
        }
    }

    // ============================================
    // VISUAL FEEDBACK
    // ============================================

    void ShowClickMarker(Vector3 position)
    {
        if (clickMarkerPrefab == null) return;

        Vector3 markerPos = position + Vector3.up * markerYOffset;
        GameObject marker = Instantiate(clickMarkerPrefab, markerPos, Quaternion.identity);
        Destroy(marker, markerLifetime);
    }

    // ============================================
    // DEBUG HELPERS
    // ============================================

    string GetPathName(PathData path)
    {
        if (path == frontPath) return "FRONT";
        if (path == middlePath) return "MIDDLE";
        if (path == backPath) return "BACK";
        return "UNKNOWN";
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        GUI.color = Color.yellow;
        GUI.Label(new Rect(10, 10, 400, 20), $"Current Path: {GetPathName(currentPath)}");
        GUI.Label(new Rect(10, 30, 400, 20), $"Status: {debugMessage}");
        GUI.Label(new Rect(10, 50, 400, 20), $"Moving: {isMoving} | Transitioning: {isTransitioning}");
        
        if (lastClickPosition != Vector3.zero)
        {
            float zDiff = lastClickPosition.z - transform.position.z;
            bool inGap = IsInTransitionGap(lastClickPosition.x);
            
            GUI.Label(new Rect(10, 70, 400, 20), $"Last Click: X={lastClickPosition.x:F2}, Z={lastClickPosition.z:F2}");
            GUI.Label(new Rect(10, 90, 400, 20), $"Z Diff: {zDiff:F2} (need {depthThreshold})");
            GUI.Label(new Rect(10, 110, 400, 20), $"In Gap: {inGap} | Can Transition: {ShouldTransition(lastClickPosition)}");
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebugInfo) return;

        // Draw transition gaps
        Gizmos.color = Color.cyan;
        
        // Left gap
        Vector3 leftGapCenter = new Vector3((LEFT_GAP_MIN + LEFT_GAP_MAX) / 2, 0.1f, transform.position.z);
        Vector3 leftGapSize = new Vector3(LEFT_GAP_MAX - LEFT_GAP_MIN, 0.1f, 10f);
        Gizmos.DrawWireCube(leftGapCenter, leftGapSize);
        
        // Right gap
        Vector3 rightGapCenter = new Vector3((RIGHT_GAP_MIN + RIGHT_GAP_MAX) / 2, 0.1f, transform.position.z);
        Vector3 rightGapSize = new Vector3(RIGHT_GAP_MAX - RIGHT_GAP_MIN, 0.1f, 10f);
        Gizmos.DrawWireCube(rightGapCenter, rightGapSize);

        // Draw last click position
        if (lastClickPosition != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastClickPosition, 0.3f);
        }
    }
}