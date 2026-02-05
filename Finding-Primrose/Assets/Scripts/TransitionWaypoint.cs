using UnityEngine;

public class TransitionWaypoint : MonoBehaviour
{
    [Header("Interaction")]
    public InteractionController interaction;
    public Transform focusPoint;

    [Header("Transition")]
    public string sceneToLoad;
    public GameObject transitionUI;
    public CanvasGroup transitionCanvas;
    public SceneTransitionController transitionController;

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

        interaction.BeginInteraction(
            transitionUI,
            transitionCanvas,
            focusPoint
        );
    }

    public void CheckExit(Vector3 dogPosition)
    {
        if (!triggered) return;

        float distance = Vector3.Distance(dogPosition, transform.position);
        if (distance > triggerRadius)
        {
            triggered = false;
        }
    }

    public void ConfirmTransition()
    {
        Debug.Log("TRANSITION BUTTON CLICKED");
        Debug.Log($"Transitioning to scene: {sceneToLoad}");
        transitionController.StartSceneTransition(sceneToLoad);
    }

    public void ResetTrigger()
    {
        triggered = false;
    }
}
