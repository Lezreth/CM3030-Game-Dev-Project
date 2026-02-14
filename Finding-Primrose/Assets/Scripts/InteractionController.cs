using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class InteractionController : MonoBehaviour
{
    [Header("Primrose")]
    public PathFollower playerPathFollower;

    //Private variables 
    private ChoiceWaypoint currentWaypoint;
    private bool interactionActive = false;
    private GameObject activeUI;
    private CanvasGroup activeCanvas;


    public void BeginInteraction(ChoiceWaypoint source)
    {
        currentWaypoint = source;

        BeginInteraction(
            source.choiceUI,
            source.choiceCanvas,
            source.npcFocusPoint
        );
    }

    public void ReturnToExploration()
    {
        if (activeUI != null)
            activeUI.SetActive(false);

        activeUI = null;
        activeCanvas = null;

        if (playerPathFollower != null)
            playerPathFollower.UnlockMovement();

        if (CameraController.Instance != null)
            CameraController.Instance.ClearFocus();

        interactionActive = false;
    }

    public void ChooseOption(int optionIndex)
    {
        if (currentWaypoint == null) return;

        if (optionIndex == 0 && currentWaypoint.choiceA != null)
            StatsManager.I.ApplyChoice(currentWaypoint.choiceA);

        if (optionIndex == 1 && currentWaypoint.choiceB != null)
            StatsManager.I.ApplyChoice(currentWaypoint.choiceB);

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

    public void BeginInteraction(
     GameObject ui,
     CanvasGroup canvas,
     Transform focusPoint
 )
    {
        if (interactionActive) return;
        interactionActive = true;

        activeUI = ui;
        activeCanvas = canvas;

        if (playerPathFollower != null)
            playerPathFollower.LockMovement();

        if (ui != null)
            ui.SetActive(true);

        if (CameraController.Instance != null && focusPoint != null)
            CameraController.Instance.FocusOn(focusPoint);

        StartCoroutine(FadeInChoices(canvas));
    }
}