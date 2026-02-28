using UnityEngine;
using System.Collections.Generic;

public class ChoiceWaypoint : MonoBehaviour
{
    private static HashSet<string> usedGroups = new HashSet<string>();

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

    public GameObject npcObject;
    public float triggerRadius = 1.5f;

    private bool triggered = false;
    private bool waitingForExit = false;
    private bool used = false;

    public bool CanTrigger(Vector3 playerPosition)
    {
        if (used) return false;
        if (waitingForExit) return false;
        if (!string.IsNullOrEmpty(interactionGroup) && usedGroups.Contains(interactionGroup))
            return false;
        if (triggered) return false;
        return Vector3.Distance(playerPosition, transform.position) <= triggerRadius;
    }

    public void Trigger()
    {
        if (triggered) return;
        triggered = true;

        if (npcObject != null)
        {
            ChefPatrolRoutine patrol = npcObject.GetComponent<ChefPatrolRoutine>();
            if (patrol != null)
            {
                Debug.Log("[ChoiceWaypoint] Stopping chef patrol");
                patrol.StopPatrol();
            }
        }

        if (interaction != null)
        {
            interaction.BeginInteraction(this);
        }
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
        used = true;

        if (!string.IsNullOrEmpty(interactionGroup))
        {
            usedGroups.Add(interactionGroup);
        }
    }
}


