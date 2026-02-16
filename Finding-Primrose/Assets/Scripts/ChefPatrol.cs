using System.Collections;
using UnityEngine;

public class ChefPatrolRoutine : MonoBehaviour
{
    [Header("Waypoints (in order)")]
    public Transform tableWaypoint;
    public Transform centerWaypoint;
    public Transform truckWaypoint;

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.5f;
    public float waitAtTruckMin = 10f;
    public float waitAtTruckMax = 20f;

    [Header("Animation")]
    public Animator animator;

    private bool isActive = true;
    private int currentWaypointIndex = 0;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
{
    while (isActive)
    {
        // 1. Walk to table
        Debug.Log("[Chef] Walking to table...");
        yield return WalkToWaypoint(tableWaypoint);
        Debug.Log("[Chef] Arrived at table. Cleaning...");
        
        // 2. Clean table
        animator.SetTrigger("DoCleanTable");
        yield return new WaitForSeconds(3f);
        Debug.Log("[Chef] Finished cleaning.");

        // 3. Walk to center
        Debug.Log("[Chef] Walking to center...");
        yield return WalkToWaypoint(centerWaypoint);
        Debug.Log("[Chef] Arrived at center. Sweeping...");
        
        // 4. Sweep
        animator.SetTrigger("DoSweep");
        yield return new WaitForSeconds(4f);
        Debug.Log("[Chef] Finished sweeping.");

        // 5. Walk to truck
        Debug.Log("[Chef] Walking to truck...");
        yield return WalkToWaypoint(truckWaypoint);
        Debug.Log("[Chef] Arrived at truck. Talking...");
        
        // 6. Talk idle
        animator.SetTrigger("DoTalk");
        
        // 7. Wait 10-20 seconds
        float waitTime = Random.Range(waitAtTruckMin, waitAtTruckMax);
        Debug.Log($"[Chef] Waiting {waitTime:F1} seconds at truck...");
        yield return new WaitForSeconds(waitTime);
        
        Debug.Log("[Chef] Starting patrol loop again!");
    }
}
IEnumerator WalkToWaypoint(Transform waypoint)
{
    if (waypoint == null)
    {
        Debug.LogError("[Chef] Waypoint is NULL!");
        yield break;
    }

    Debug.Log($"[Chef] Starting walk. Distance to waypoint: {Vector3.Distance(transform.position, waypoint.position):F2}m");
    
    // Start walk animation
    animator.SetBool("Walking", true);

    int frameCount = 0;
    while (Vector3.Distance(transform.position, waypoint.position) > stoppingDistance)
    {
        frameCount++;
        
        // Debug every 60 frames (about once per second)
        if (frameCount % 60 == 0)
        {
            float dist = Vector3.Distance(transform.position, waypoint.position);
            Debug.Log($"[Chef] Still walking... Distance: {dist:F2}m, Stopping at: {stoppingDistance}m");
        }

        // Move toward waypoint
        Vector3 direction = (waypoint.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Rotate to face direction
        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        yield return null;
    }

    Debug.Log($"[Chef] ARRIVED! Final distance: {Vector3.Distance(transform.position, waypoint.position):F2}m");
    
    // Stop walk animation
    animator.SetBool("Walking", false);
}

    //  when player triggers the choice
    public void StopPatrol()
    {
        isActive = false;
        StopAllCoroutines();
        animator.SetBool("Walking", false);
    }
}