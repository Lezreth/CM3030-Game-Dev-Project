using UnityEngine;
using UnityEngine.InputSystem;

public class PathFollower : MonoBehaviour
{
    [Header("Paths")]
    public PathData frontPath;
    public PathData middlePath;
    public PathData backPath;
    public PathData backBackPath; //4th path if desired

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

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (currentPath == null)
            currentPath = frontPath;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        // ADD THIS DEBUG
        Debug.Log($"[PathFollower] Animator found: {(animator != null ? "YES" : "NO")}");
        if (animator != null)
        {
            Debug.Log($"[PathFollower] Animator name: {animator.name}");
            Debug.Log($"[PathFollower] Animator has 'Walk' parameter: {HasParameter(animator, "Walk")}");
        }
    }

    // ADD THIS HELPER METHOD
    bool HasParameter(Animator anim, string paramName)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
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
        
        if (isTransitionClick)
        {
            processedClick = ClampToHallway(hit.point);
            Debug.Log($"Transition click: Original X={hit.point.x:F2} → Clamped X={processedClick.x:F2}");
        }

        // Try path transition first
        if (TryStartPathTransition(processedClick)) return;

        MoveOnCurrentPath(processedClick);
    }

    Vector3 ClampToHallway(Vector3 clickPosition)
    {
        Vector3 clamped = clickPosition;
        float x = clickPosition.x;

        // Determine which hallway based on X position
        if (x < 0)
        {
            // LEFT SIDE - check both inner and outer hallways
            
            // Left outer hallway: -10.5 to -8
            if (x <= leftOuterHallwayInner)
            {
                if (x < leftOuterHallwayOuter)
                {
                    clamped.x = leftOuterHallwayOuter;
                    debugMessage = "Clamped to left outer hallway outer";
                }
                else if (x > leftOuterHallwayInner)
                {
                    clamped.x = leftOuterHallwayInner;
                    debugMessage = "Clamped to left outer hallway inner";
                }
                else
                {
                    debugMessage = "In left outer hallway";
                }
            }
            // Left inner hallway: -3.3 to -2.1
            else
            {
                if (x < leftInnerHallwayOuter)
                {
                    clamped.x = leftInnerHallwayOuter;
                    debugMessage = "Clamped to left inner hallway outer";
                }
                else if (x > leftInnerHallwayInner)
                {
                    clamped.x = leftInnerHallwayInner;
                    debugMessage = "Clamped to left inner hallway inner";
                }
                else
                {
                    debugMessage = "In left inner hallway";
                }
            }
        }
        else if (x > 0)
        {
         
     
            if (x >= rightOuterHallwayInner)
            {
                if (x > rightOuterHallwayOuter)
                {
                    clamped.x = rightOuterHallwayOuter;
                    debugMessage = "Clamped to right outer hallway outer";
                }
                else if (x < rightOuterHallwayInner)
                {
                    clamped.x = rightOuterHallwayInner;
                    debugMessage = "Clamped to right outer hallway inner";
                }
                else
                {
                    debugMessage = "In right outer hallway";
                }
            }
           
            else
            {
                if (x < rightInnerHallwayInner)
                {
                    clamped.x = rightInnerHallwayInner;
                    debugMessage = "Clamped to right inner hallway inner";
                }
                else if (x > rightInnerHallwayOuter)
                {
                    clamped.x = rightInnerHallwayOuter;
                    debugMessage = "Clamped to right inner hallway outer";
                }
                else
                {
                    debugMessage = "In right inner hallway";
                }
            }
        }

        return clamped;
    }

    bool TryStartPathTransition(Vector3 clickPosition)
    {
        if (!ShouldTransition(clickPosition))
        {
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

        TransitionToPath(targetPath, clickPosition.x);
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
    
void MoveToTarget()
{
    Vector3 currentPos = transform.position;
    Vector3 targetPos = targetPosition;
    
    Vector3 moveDirection = targetPos - currentPos;
    moveDirection.y = 0;
    
    float distanceToTarget = moveDirection.magnitude;

    if (distanceToTarget > stoppingDistance)
    {
        
        Debug.Log($"[Animation] Moving! Distance: {distanceToTarget:F2}");
        Debug.Log($"[Animation] Animator null? {(animator == null)}");
        
        if (animator != null)
        {
            animator.SetBool("Walk", true);
            Debug.Log("[Animation] Set Walk = TRUE");
        }
        else
        {
            Debug.LogError("[Animation] ANIMATOR IS NULL!");
        }
            
        Vector3 newPosition = Vector3.MoveTowards(
            currentPos,
            targetPos,
            moveSpeed * Time.deltaTime
        );
        
        transform.position = newPosition;

        if (moveDirection.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    else
    {
        
        Debug.Log("[Animation] Stopped - Set Walk = FALSE");
        
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
        
        // Stop animation when locked
        if (animator != null)
            animator.SetBool("Walk", false);  
    }

    public void UnlockMovement()
    {
        movementLocked = false;
    }
    
    bool ShouldTransition(Vector3 clickPosition)
    {
        float zDifference = clickPosition.z - transform.position.z;
        
        Debug.Log($"[ShouldTransition] Z diff: {Mathf.Abs(zDifference):F2}, Threshold: {depthThreshold}");
        Debug.Log($"[ShouldTransition] Meets threshold: {(Mathf.Abs(zDifference) >= depthThreshold)}");

        if (Mathf.Abs(zDifference) < depthThreshold)
        {
            Debug.Log("[ShouldTransition] FAILED - Not deep enough");
            return false;
        }

       
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
        // Check all 4 hallways
        bool inLeftInner = xPosition >= leftInnerHallwayOuter && xPosition <= leftInnerHallwayInner;
        bool inRightInner = xPosition >= rightInnerHallwayInner && xPosition <= rightInnerHallwayOuter;
        bool inLeftOuter = xPosition >= leftOuterHallwayOuter && xPosition <= leftOuterHallwayInner;
        bool inRightOuter = xPosition >= rightOuterHallwayInner && xPosition <= rightOuterHallwayOuter;
        
        return inLeftInner || inRightInner || inLeftOuter || inRightOuter;
    }
PathData GetTargetPath(Vector3 clickPosition)
{
    float zDifference = clickPosition.z - transform.position.z;
    bool clickingBackward = zDifference > 0;

    string currentPathName = GetPathName(currentPath);
    
    Debug.Log("════════════════════════════════════════");
    Debug.Log($"GetTargetPath CALLED");
    Debug.Log($"Current: {currentPathName}");
    Debug.Log($"Player Z: {transform.position.z:F2}");
    Debug.Log($"Click Z: {clickPosition.z:F2}");
    Debug.Log($"Z diff: {zDifference:F2}");
    Debug.Log($"Clicking: {(clickingBackward ? "BACKWARD" : "FORWARD")}");
    Debug.Log($"Middle Path assigned: {(middlePath != null ? "YES" : "NULL")}");
    Debug.Log($"Back Path assigned: {(backPath != null ? "YES" : "NULL")}");
    Debug.Log($"BackBack Path assigned: {(backBackPath != null ? "YES" : "NULL")}");

    // Front → Middle (backward only)
    if (currentPath == frontPath)
    {
        if (clickingBackward && middlePath != null)
        {
            Debug.Log("✓ Transitioning: Front → Middle");
            return middlePath;
        }
        Debug.Log("✗ Can't transition from Front (either not backward or middlePath null)");
        return null;
    }

    // Middle → Front or Back
    if (currentPath == middlePath)
    {
        if (clickingBackward && backPath != null)
        {
            Debug.Log("✓ Transitioning: Middle → Back");
            return backPath;
        }
        else if (!clickingBackward && frontPath != null)
        {
            Debug.Log("✓ Transitioning: Middle → Front");
            return frontPath;
        }
        Debug.Log("✗ Can't transition from Middle");
        return null;
    }

    // Back → Middle or BackBack
    if (currentPath == backPath)
    {
        if (clickingBackward && backBackPath != null)
        {
            Debug.Log("✓ Transitioning: Back → BackBack");
            return backBackPath;
        }
        else if (!clickingBackward && middlePath != null)
        {
            Debug.Log("✓ Transitioning: Back → Middle");
            return middlePath;
        }
        Debug.Log("✗ Can't transition from Back");
        return null;
    }

    // BackBack → Back (forward only)
    if (currentPath == backBackPath)
    {
        if (!clickingBackward && backPath != null)
        {
            Debug.Log("✓ Transitioning: BackBack → Back");
            return backPath;
        }
        Debug.Log("✗ Can't transition from BackBack");
        return null;
    }

    Debug.Log("✗ Current path doesn't match any known path!");
    Debug.Log("════════════════════════════════════════");
    return null;
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
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    if (progress >= 1f)
    {
        // ADD THIS - Stop walking when transition completes
        if (animator != null)
            animator.SetBool("Walk", false);
            
        isTransitioning = false;
        transform.position = transitionEndPos;
        Debug.Log($"Transition complete. Now on: {GetPathName(currentPath)}");
    
    }
    }

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

    void ShowClickMarker(Vector3 position)
    {
        if (clickMarkerPrefab == null) return;

        Vector3 markerPos = position + Vector3.up * markerYOffset;
        GameObject marker = Instantiate(clickMarkerPrefab, markerPos, Quaternion.identity);
        Destroy(marker, markerLifetime);
    }

    string GetPathName(PathData path)
    {
        if (path == frontPath) return "FRONT";
        if (path == middlePath) return "MIDDLE";
        if (path == backPath) return "BACK";
        if (path == backBackPath) return "BACK-BACK";
        return "UNKNOWN";
    }

    // void OnGUI()
    // {
    //     if (!showDebugInfo) return;

    //     GUI.color = Color.yellow;
    //     GUI.Label(new Rect(10, 10, 400, 20), $"Current Path: {GetPathName(currentPath)}");
    //     GUI.Label(new Rect(10, 30, 400, 20), $"Status: {debugMessage}");
    //     GUI.Label(new Rect(10, 50, 400, 20), $"Moving: {isMoving} | Transitioning: {isTransitioning}");
        
    //     GUI.Label(new Rect(10, 70, 400, 20), $"Left Inner: [{leftInnerHallwayOuter:F2} to {leftInnerHallwayInner:F2}]");
    //     GUI.Label(new Rect(10, 90, 400, 20), $"Right Inner: [{rightInnerHallwayInner:F2} to {rightInnerHallwayOuter:F2}]");
    //     GUI.Label(new Rect(10, 110, 400, 20), $"Left Outer: [{leftOuterHallwayOuter:F2} to {leftOuterHallwayInner:F2}]");
    //     GUI.Label(new Rect(10, 130, 400, 20), $"Right Outer: [{rightOuterHallwayInner:F2} to {rightOuterHallwayOuter:F2}]");
        
    //     if (isTransitioning)
    //     {
    //         GUI.Label(new Rect(10, 150, 400, 20), $"Hallway X: {transitionHallwayX:F2}");
    //     }
    // }

    // void OnDrawGizmos()
    // {
    //     if (!showBoundaries) return;

    //     float zPos = transform.position.z;
    //     float zLength = 30f;
        
    //     // Left inner hallway (cyan)
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawLine(
    //         new Vector3(leftInnerHallwayOuter, 0.5f, zPos - zLength/2),
    //         new Vector3(leftInnerHallwayOuter, 0.5f, zPos + zLength/2)
    //     );
    //     Gizmos.DrawLine(
    //         new Vector3(leftInnerHallwayInner, 0.5f, zPos - zLength/2),
    //         new Vector3(leftInnerHallwayInner, 0.5f, zPos + zLength/2)
    //     );
        
    //     // Right inner hallway (cyan)
    //     Gizmos.DrawLine(
    //         new Vector3(rightInnerHallwayInner, 0.5f, zPos - zLength/2),
    //         new Vector3(rightInnerHallwayInner, 0.5f, zPos + zLength/2)
    //     );
    //     Gizmos.DrawLine(
    //         new Vector3(rightInnerHallwayOuter, 0.5f, zPos - zLength/2),
    //         new Vector3(rightInnerHallwayOuter, 0.5f, zPos + zLength/2)
    //     );

    //     // Left outer hallway (magenta)
    //     Gizmos.color = Color.magenta;
    //     Gizmos.DrawLine(
    //         new Vector3(leftOuterHallwayOuter, 0.5f, zPos - zLength/2),
    //         new Vector3(leftOuterHallwayOuter, 0.5f, zPos + zLength/2)
    //     );
    //     Gizmos.DrawLine(
    //         new Vector3(leftOuterHallwayInner, 0.5f, zPos - zLength/2),
    //         new Vector3(leftOuterHallwayInner, 0.5f, zPos + zLength/2)
    //     );
        
    //     // Right outer hallway (magenta)
    //     Gizmos.DrawLine(
    //         new Vector3(rightOuterHallwayInner, 0.5f, zPos - zLength/2),
    //         new Vector3(rightOuterHallwayInner, 0.5f, zPos + zLength/2)
    //     );
    //     Gizmos.DrawLine(
    //         new Vector3(rightOuterHallwayOuter, 0.5f, zPos - zLength/2),
    //         new Vector3(rightOuterHallwayOuter, 0.5f, zPos + zLength/2)
    //     );

    //     // Draw transition path
    //     if (isTransitioning)
    //     {
    //         Gizmos.color = Color.yellow;
    //         Gizmos.DrawLine(
    //             new Vector3(transitionHallwayX, 0.5f, transitionStartPos.z),
    //             new Vector3(transitionHallwayX, 0.5f, transitionEndPos.z)
    //         );
    //     }

    //     // Draw last click
    //     if (lastClickPosition != Vector3.zero)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawSphere(lastClickPosition, 0.3f);
    //     }
        
    //     // Draw current target
    //     if (isMoving && targetPosition != Vector3.zero)
    //     {
    //         Gizmos.color = Color.white;
    //         Gizmos.DrawSphere(targetPosition, 0.2f);
    //         Gizmos.DrawLine(transform.position, targetPosition);
    //     }
    // }
}