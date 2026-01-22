using UnityEngine;

public class ChoiceWaypoint : MonoBehaviour
{
    public InteractionController interaction;
    public float triggerRadius = 1.5f;
    private bool triggered = false;

    public bool CanTrigger(Vector3 dogPosition)
    {
        if (triggered) return false;
        return Vector3.Distance(dogPosition, transform.position) <= triggerRadius;
    }

    public void Trigger()
    {
        if (triggered) return;
        triggered = true;

        if (interaction != null)
        {
            interaction.BeginInteraction();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}


