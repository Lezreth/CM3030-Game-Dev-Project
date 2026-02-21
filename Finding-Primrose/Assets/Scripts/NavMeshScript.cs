using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NavMeshScript : MonoBehaviour
{
  
    private NavMeshAgent agent;
    public Camera mainCamera;

    [SerializeField] private ChoiceWaypoint[] waypoints; 
    
    [SerializeField] private LayerMask walkableLayer; 
    [SerializeField] private Animator animator; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
         CheckWaypointProximity();
        
        bool isMoving = agent.velocity.magnitude > 0.1f;
        if (animator != null)
            animator.SetBool("Walk", isMoving);

        // if (isMoving)
        // {
        //     Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        // }
        // Rotate to face movement direction
        if (agent.hasPath && !agent.isStopped && agent.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (agent.hasPath && !agent.isStopped)
        {
            Debug.Log($"[MOVING] Actual pos: {transform.position} | Destination: {agent.destination} | Remaining: {agent.remainingDistance}");
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, walkableLayer))
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(hit.point, path);
            Debug.Log($"[CLICK] Destination: {hit.point}");
            Debug.Log($"[PATH] Status: {path.status}");

            UnityEngine.AI.NavMeshHit navSample;
            bool sampled = UnityEngine.AI.NavMesh.SamplePosition(hit.point, out navSample, 2f, UnityEngine.AI.NavMesh.AllAreas);
            Debug.Log($"[NAVMESH SAMPLE] Exists: {sampled} | Nearest point: {navSample.position}");

            agent.SetDestination(hit.point);
        }
    }

    void CheckWaypointProximity()
    {
        if (waypoints == null) return;
        
        foreach (ChoiceWaypoint waypoint in waypoints)
        {
            if (waypoint == null) continue;
            
            waypoint.CheckExit(transform.position);
            
            if (waypoint.CanTrigger(transform.position))
            {
                Debug.Log($"[WAYPOINT] Triggering: {waypoint.name}");
                waypoint.Trigger();
                
                // Stop the agent when interaction begins
                agent.isStopped = true;
                agent.ResetPath();
            }
        }
    }
}