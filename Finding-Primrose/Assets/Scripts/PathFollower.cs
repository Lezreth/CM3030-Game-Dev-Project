// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PathFollower : MonoBehaviour
// {


//     public PathData frontPath;
//     public PathData middlePath;
//     public PathData backPath;
//     public PathData currentPath;
//     public GameObject clickMarkerPrefab;
//     public Camera mainCamera;
   
    
   
//     public float moveSpeed = 3f;
//     public float rotationSpeed = 5f;
//     public float stoppingDistance = 0.1f;
    
//     public float markerLifetime = 0.5f;
//     public float markerYOffset = 0.1f;
    
  
//     public float transitionZThreshold = -1f; // Z value to trigger path switch
//     public float transitionDuration = 0.5f;
    
//     private Vector3 targetPosition;
//     private bool isMoving = false;
//     private bool isTransitioning = false;
//     private float transitionTimer = 0f;

//     private Vector3 lastClickPosition;
//     private string debugInfo = "Click to see position";
//     private Vector3 transitionStartPos;
//     private Vector3 transitionEndPos;
    
//     void Start()
//     {
//         if (mainCamera == null)
//             mainCamera = Camera.main;
            
//         // Start on front path by default
//         if (currentPath == null)
//             currentPath = frontPath;
//     }
    
//     void Update()
//     {
//         // Handle path transitions
//         if (isTransitioning)
//         {
//             HandleTransition();
//             return;
//         }
        
//         // Handle mouse click
//         if (Mouse.current.leftButton.wasPressedThisFrame)
//         {
//             HandleClick();
//         }
                
//         // Move character
//         if (isMoving)
//         {
//             MoveToTarget();
//         }
//     }
    
//     void HandleClick()
//     {
//         Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
//         RaycastHit hit;
        
//         // Raycast to find click position
//         if (Physics.Raycast(ray, out hit, 100f))
//         {
//             //store last click position for debug
//             lastClickPosition = hit.point;
//             debugInfo = $"Click: X={hit.point.x:F2}, Y={hit.point.y:F2}, Z={hit.point.z:F2}";
            
//             // Check if this click should trigger a transition
//             if (ShouldTransition(hit.point))
//             {
//                 // Determine which path to transition to
//                 PathData targetPath = DeterminePathFromClick(hit.point);
                
//                 if (targetPath != currentPath && targetPath != null)
//                 {
//                     TransitionToPath(targetPath);
//                     return; // Don't set movement target during transition
//                 }
//             }
//              else
//             {
//                 debugInfo = "Click missed - no collider!";
//             }
            
//             // Normal movement on current path
//             if (currentPath != null)
//             {
//                 targetPosition = currentPath.GetClosestPointOnPath(hit.point);
//                 isMoving = true;
//                 ShowClickMarker(targetPosition);
//             }
//         }
//             }
//         bool ShouldTransition(Vector3 clickPosition)
//         {
//             // Calculate Z difference between click and dog
//             float zDifference = clickPosition.z - transform.position.z;
            
//             // Only transition if clicking significantly forward or backward
//             float depthThreshold = 1f;
            
//             if (Mathf.Abs(zDifference) < depthThreshold)
//             {
//                 // Click is roughly at dog's depth - just move horizontally
//                 return false;
//             }
            
//             // Check if click is in valid transition zones (gaps between containers)
//             bool inLeftGap = clickPosition.x >= -3.75f && clickPosition.x <= -1.75f;
//             bool inRightGap = clickPosition.x >= 1.75f && clickPosition.x <= 3.75f;
            
//             return inLeftGap || inRightGap;
//         }

//         PathData DeterminePathFromClick(Vector3 clickPosition)
//         {
//             // Compare click position and primrose current position
//             float zDifference = clickPosition.z - transform.position.z;
            
//             // Determine direction
//             bool clickingForward = zDifference < 0;
//             bool clickingBackward = zDifference > 0;
            
//             // Transition to next path based on current path and click direction
//             if (currentPath == frontPath)
//             {
//                 if (clickingBackward)
//                     return middlePath; // Front → Middle
//                 else
//                     return frontPath; // Stay on front
//             }
//             else if (currentPath == middlePath)
//             {
//                 if (clickingBackward)
//                     return backPath; // Middle → Back
//                 else if (clickingForward)
//                     return frontPath; // Middle → Front
//                 else
//                     return middlePath; // Stay on middle
//             }
//             else if (currentPath == backPath)
//             {
//                 if (clickingForward)
//                     return middlePath; // Back → Middle
//                 else
//                     return backPath; // Stay on back
//             }
            
//             // Fallback
//             return currentPath;
//         }
    
//     void MoveToTarget()
//     {
//         // Calculate direction
//         Vector3 direction = (targetPosition - transform.position);
//         direction.y = 0; // Keep on same y-level
        
//         // Move toward target
//         transform.position = Vector3.MoveTowards(
//             transform.position,
//             targetPosition,
//             moveSpeed * Time.deltaTime
//         );
        
//         // turn toward movement direction
//         if (direction.magnitude > 0.1f)
//         {
//             direction.Normalize();
//             Quaternion targetRotation = Quaternion.LookRotation(direction);
//             transform.rotation = Quaternion.Slerp(
//                 transform.rotation,
//                 targetRotation,
//                 rotationSpeed * Time.deltaTime
//             );
//         }
        
//         // Stop when point reached
//         if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
//         {
//             isMoving = false;
//         }
//     }
    
//     void ShowClickMarker(Vector3 position)
//     {
//         if (clickMarkerPrefab != null)
//         {
//             Vector3 markerPos = position;
//             markerPos.y += markerYOffset;
            
//             GameObject marker = Instantiate(clickMarkerPrefab, markerPos, Quaternion.identity);
//             Destroy(marker, markerLifetime);
//         }
//     }
    
//     void TransitionToPath(PathData newPath)
//     {
//         if (newPath == null || isTransitioning)
//             return;
            
//         // Find closest point on new path
//         Vector3 newPathPoint = newPath.GetClosestPointOnPath(transform.position);
        
//         // Start transition
//         isTransitioning = true;
//         isMoving = false;
//         transitionTimer = 0f;
//         transitionStartPos = transform.position;
//         transitionEndPos = newPathPoint;
        
//         // Switch path
//         currentPath = newPath;
//     }
    
//     void HandleTransition()
//     {
//         transitionTimer += Time.deltaTime;
//         float progress = transitionTimer / transitionDuration;
        
//         // move between paths
//         transform.position = Vector3.Lerp(transitionStartPos, transitionEndPos, progress);
        
//         // End transition
//         if (progress >= 1f)
//         {
//             isTransitioning = false;
//             transform.position = transitionEndPos;
//         }
//     }

//   void OnGUI()
//     {
//         GUI.contentColor = Color.yellow;
//         GUI.Label(new Rect(10, 10, 500, 20), debugInfo);
        
//         if (lastClickPosition != Vector3.zero)
//         {
//             float zDiff = lastClickPosition.z - transform.position.z;
//             bool shouldTrans = ShouldTransition(lastClickPosition);
            
//             string currentPathName = currentPath == frontPath ? "Front" : 
//                                     currentPath == middlePath ? "Middle" : "Back";
            
//             GUI.Label(new Rect(10, 30, 500, 20), $"Current Path: {currentPathName}");
//             GUI.Label(new Rect(10, 50, 500, 20), $"Z Difference: {zDiff:F2} (Threshold: 1.5)");
//             GUI.Label(new Rect(10, 70, 500, 20), $"Should Transition: {shouldTrans}");
//         }
//     }
// }



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
    
    [Header("Marker (aka bone)")]
    public float markerLifetime = 0.5f;
    public float markerYOffset = 0.1f;
    
    [Header("Transition Settings")]
    public float depthThreshold = 1.5f; // How far forward/back to trigger transition
    public float transitionDuration = 0.5f;
    
    // Private variables
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isTransitioning = false;
    private float transitionTimer = 0f;
    private Vector3 transitionStartPos;
    private Vector3 transitionEndPos;
    
    // Debug
    private Vector3 lastClickPosition;
    private string debugInfo = "Click to see position";
    
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
            
        if (currentPath == null)
            currentPath = frontPath;
    }
    
    void Update()
    {
        if (isTransitioning)
        {
            HandleTransition();
            return;
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
                
        if (isMoving)
        {
            MoveToTarget();
        }
    }
    
    void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100f))
        {
            lastClickPosition = hit.point;
            debugInfo = $"Click: X={hit.point.x:F2}, Y={hit.point.y:F2}, Z={hit.point.z:F2}";
            
            if (ShouldTransition(hit.point))
            {
                PathData targetPath = DeterminePathFromClick(hit.point);
                
                // Debug logging
                string targetPathName = targetPath == frontPath ? "Front" : 
                                       targetPath == middlePath ? "Middle" : 
                                       targetPath == backPath ? "Back" : "NULL";
                Debug.Log($"Attempting transition to: {targetPathName}");
                
                if (targetPath != null && targetPath != currentPath)
                {
                    TransitionToPath(targetPath);
                    return;
                }
                else if (targetPath == null)
                {
                    Debug.LogError("Target path is NULL! Check Inspector assignments.");
                }
            }
            
            // Normal movement on current path
            if (currentPath != null)
            {
                targetPosition = currentPath.GetClosestPointOnPath(hit.point);
                isMoving = true;
                ShowClickMarker(targetPosition);
            }
        }
        else
        {
            debugInfo = "Click missed - no collider!";
        }
    }
    
    bool ShouldTransition(Vector3 clickPosition)
    {
        float zDifference = clickPosition.z - transform.position.z;
        
        // Must click far enough forward/back
        if (Mathf.Abs(zDifference) < depthThreshold)
            return false;
        
        // Must click in a gap between containers
        bool inLeftGap = clickPosition.x >= -3.75f && clickPosition.x <= -1.75f;
        bool inRightGap = clickPosition.x >= 1.75f && clickPosition.x <= 3.75f;
        
        return inLeftGap || inRightGap;
    }
    
    PathData DeterminePathFromClick(Vector3 clickPosition)
    {
        float zDifference = clickPosition.z - transform.position.z;
        
        bool clickingBackward = zDifference > 0;
        bool clickingForward = zDifference < 0;
        
        // Front path logic
        if (currentPath == frontPath)
        {
            return clickingBackward ? middlePath : frontPath;
        }
        
        // Middle path logic
        if (currentPath == middlePath)
        {
            if (clickingBackward) return backPath;
            if (clickingForward) return frontPath;
            return middlePath;
        }
        
        // Back path logic
        if (currentPath == backPath)
        {
            return clickingForward ? middlePath : backPath;
        }
        
        return currentPath;
    }
    
    void MoveToTarget()
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0;
        
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
        
        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            isMoving = false;
        }

        //Checks for ChoiceWaypoint
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            isMoving = false;
            CheckForChoiceWaypoint();
        }

    }

    void ShowClickMarker(Vector3 position)
    {
        if (clickMarkerPrefab != null)
        {
            Vector3 markerPos = position + Vector3.up * markerYOffset;
            GameObject marker = Instantiate(clickMarkerPrefab, markerPos, Quaternion.identity);
            Destroy(marker, markerLifetime);
        }
    }
    
    void TransitionToPath(PathData newPath)
    {
        if (newPath == null || isTransitioning)
            return;
            
        Vector3 newPathPoint = newPath.GetClosestPointOnPath(transform.position);
        
        isTransitioning = true;
        isMoving = false;
        transitionTimer = 0f;
        transitionStartPos = transform.position;
        transitionEndPos = newPathPoint;
        
        currentPath = newPath;
    }
    
    void HandleTransition()
    {
        transitionTimer += Time.deltaTime;
        float progress = transitionTimer / transitionDuration;
        
        transform.position = Vector3.Lerp(transitionStartPos, transitionEndPos, progress);
        
        if (progress >= 1f)
        {
            isTransitioning = false;
            transform.position = transitionEndPos;
        }
    }

    // tells ChoiceWaypoint script to trigger choice system. 
    void CheckForChoiceWaypoint()
    {
        if (currentPath == null || currentPath.waypoints == null)
            return;

        foreach (Transform wp in currentPath.waypoints)
        {
            if (wp == null) continue;

            ChoiceWaypoint choice = wp.GetComponent<ChoiceWaypoint>();
            if (choice != null && choice.CanTrigger(transform.position))
            {
                choice.Trigger();
                return;
            }
        }
    }

    void OnGUI()
    {
        GUI.contentColor = Color.yellow;
        GUI.Label(new Rect(10, 10, 500, 20), debugInfo);
        
        if (lastClickPosition != Vector3.zero)
        {
            float zDiff = lastClickPosition.z - transform.position.z;
            bool shouldTrans = ShouldTransition(lastClickPosition);
            
            string currentPathName = currentPath == frontPath ? "Front" : 
                                    currentPath == middlePath ? "Middle" : 
                                    currentPath == backPath ? "Back" : "Unknown";
            
            GUI.Label(new Rect(10, 30, 500, 20), $"Current Path: {currentPathName}");
            GUI.Label(new Rect(10, 50, 500, 20), $"Z Difference: {zDiff:F2} (Threshold: {depthThreshold})");
            GUI.Label(new Rect(10, 70, 500, 20), $"Should Transition: {shouldTrans}");
        }
    }
}