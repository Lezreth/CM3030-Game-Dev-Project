

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;

public class NavMeshScript : MonoBehaviour
{
    private NavMeshAgent agent;
    public Camera mainCamera;

    [SerializeField] private ChoiceWaypoint[] waypoints;
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private Animator animator;

    [Header("Click Marker")]
    [SerializeField] private GameObject clickMarkerPrefab;  
    [SerializeField] private float markerDuration = 0.8f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        CheckWaypointProximity();

        bool isMoving = agent.velocity.magnitude > 0.1f;
        //if (animator != null)
        //    animator.SetBool("Walk", isMoving);
        if (animator != null)
        {
            bool isCrouching = animator.GetBool("CautiousCrouch");

            if (isCrouching)
            {
                animator.SetBool("Walk", false); 
                animator.SetBool("CautiousWalk", isMoving);
            }
            else
            {
                animator.SetBool("CautiousWalk", false);
                animator.SetBool("Walk", isMoving);
            }
        }

        if (agent.hasPath && !agent.isStopped && agent.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, walkableLayer))
        {
            agent.SetDestination(hit.point);
            ShowMarker(hit.point);
        }
    }

    void ShowMarker(Vector3 point)
    {
        if (clickMarkerPrefab == null) return;
        GameObject marker = Instantiate(clickMarkerPrefab, point + Vector3.up * 0.02f, Quaternion.Euler(90f, 0f, 0f));
        StartCoroutine(FadeMarker(marker));
    }

    IEnumerator FadeMarker(GameObject marker)
    {
        Renderer rend = marker.GetComponent<Renderer>();
        Material mat = rend.material;

        // Fade in
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.12f;
            mat.SetColor("_BaseColor", new Color(1f, 1f, 1f, Mathf.Clamp01(t)));
            yield return null;
        }

        // hold
        yield return new WaitForSeconds(markerDuration);

        // Fade out
        t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / 0.3f;
            mat.SetColor("_BaseColor", new Color(1f, 1f, 1f, Mathf.Clamp01(t)));
            yield return null;
        }

        Destroy(marker);
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
                waypoint.Trigger();
                agent.isStopped = true;
                agent.ResetPath();
            }
        }
    }
}