using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NavMeshScript : MonoBehaviour


{
    private NavMeshAgent agent;
    public Camera mainCamera;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            agent.SetDestination(hit.point);
    }
}