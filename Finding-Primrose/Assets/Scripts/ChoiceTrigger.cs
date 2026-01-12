using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public InteractionController interaction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interaction.BeginInteraction();
        }
    }
}

