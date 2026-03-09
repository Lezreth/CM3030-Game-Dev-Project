using UnityEngine;
using System.Collections.Generic;

public class ChoiceWaypoint : MonoBehaviour
{
    
    [Header("Interaction")]
    public InteractionController interaction;
    public Transform npcFocusPoint;

    [Header("Choices")]
    public ChoiceEffect choiceA;
    public ChoiceEffect choiceB;
    public GameObject choiceUI;
    public CanvasGroup choiceCanvas;

    [Header("Linking")]
    public string interactionGroup;

    [Header("Persistence")]
    public string waypointID;

    [Header("Player Animation")]
    public PrimroseAnimationController primroseAnim;

    [Header("Animation")]
    public bool triggerCrouchOnActivate;

    public GameObject npcObject;
    public float triggerRadius = 1.5f;

    private static HashSet<string> usedGroups = new HashSet<string>();
    private static HashSet<string> usedWaypoints = new HashSet<string>();

    private bool triggered = false;
    private bool waitingForExit = false;

    public bool CanTrigger(Vector3 playerPosition)
    {
        if (waitingForExit) return false;
        if (triggered) return false;

        // looks to see what individual waypoints were used 
        if (!string.IsNullOrEmpty(waypointID) && usedWaypoints.Contains(waypointID))
            return false;

        // Used to see if any waypoints form the group was used 
        if (!string.IsNullOrEmpty(interactionGroup) && usedGroups.Contains(interactionGroup))
            return false;

        return Vector3.Distance(playerPosition, transform.position) <= triggerRadius;
    }

    public void Trigger()
    {
        if (triggered) return;
        triggered = true;

        if (primroseAnim != null && triggerCrouchOnActivate)
            primroseAnim.SetCautiousCrouch(true);

        if (npcObject != null)
        {
            ChefPatrolRoutine patrol = npcObject.GetComponent<ChefPatrolRoutine>();
            if (patrol != null)
            {
                Debug.Log("[ChoiceWaypoint] Stopping chef patrol");
                patrol.StopPatrol();
                interaction.activePatrol = patrol;  // ← inside the block where patrol exists
            }
        }

        if (interaction != null)
            interaction.BeginInteraction(this);
    }
    public void CheckExit(Vector3 playerPosition)
    {
        if (!triggered) return;

        float distance = Vector3.Distance(playerPosition, transform.position);
        if (distance > triggerRadius)
        {
            triggered = false;
        }
    }

    public void ResetTrigger()
    {
        triggered = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }

    public void MarkAsUsed()
    {
        if (!string.IsNullOrEmpty(waypointID))
        {
            usedWaypoints.Add(waypointID);
        }

        if (!string.IsNullOrEmpty(interactionGroup))
        {
            usedGroups.Add(interactionGroup);
        }
    }
}


