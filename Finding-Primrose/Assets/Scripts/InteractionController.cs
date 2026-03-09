using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class InteractionController : MonoBehaviour
{
    [Header("Primrose")]
    public NavMeshAgent playerAgent;

    private ChoiceWaypoint currentWaypoint;
    private bool interactionActive = false;
    private GameObject activeUI;
    private CanvasGroup activeCanvas;

    public ChefPatrolRoutine activePatrol;

    
    public void BeginInteraction(ChoiceWaypoint source)
    {
        currentWaypoint = source;
        BeginInteraction(source.choiceUI, source.choiceCanvas, source.npcFocusPoint);

    }

   
    public void BeginInteraction(GameObject ui, CanvasGroup canvas, Transform focusPoint)
    {
        if (interactionActive)
        {
            Debug.LogWarning("[IC] BeginInteraction called while already active");
            ReturnToExploration();
        }

        interactionActive = true;
        activeUI = ui;
        activeCanvas = canvas;

        if (playerAgent != null)
        {
            playerAgent.isStopped = true;
            playerAgent.ResetPath();
            //Debug.Log("[InteractionController] NavMesh Agent stopped");
        }

        if (ui != null)
            ui.SetActive(true);

        if (CameraController.Instance != null && focusPoint != null)
            CameraController.Instance.FocusOn(focusPoint);

        StartCoroutine(FadeInChoices(canvas));
    }

    public void ReturnToExploration()
    {
        Debug.Log($"[IC] ReturnToExploration called, wasActive: {interactionActive}");

        if (activeUI != null)
            activeUI.SetActive(false);

        activeUI = null;
        activeCanvas = null;

        if (playerAgent != null)
        {
            playerAgent.isStopped = false;
            Debug.Log("[InteractionController] NavMesh Agent resumed");
        }

        if (CameraController.Instance != null)
            CameraController.Instance.ClearFocus();

        interactionActive = false;

        if (activePatrol != null)
        {
            activePatrol.ResumePatrol();
            activePatrol = null;
        }
    }

    public void ChooseOption(int optionIndex)
    {
        if (currentWaypoint == null) return;

        ChoiceWaypoint waypoint = currentWaypoint;
        currentWaypoint = null;

        ChoiceEffect effect = optionIndex == 0 ? waypoint.choiceA : waypoint.choiceB;

        if (effect != null)
        {
            Debug.Log($"[InteractionController] Applying choice {optionIndex} from '{waypoint.name}'");
            StatsManager.I.ApplyChoice(effect);
        }
        else
        {
            Debug.LogWarning($"[InteractionController] No ChoiceEffect for option {optionIndex} on '{waypoint.name}'");
        }

        waypoint.MarkAsUsed();
        ReturnToExploration();
    }

    IEnumerator FadeInChoices(CanvasGroup canvas)
    {
        if (canvas == null) yield break;

        float t = 0f;
        float duration = 0.4f;
        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }

        canvas.alpha = 1;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }
}