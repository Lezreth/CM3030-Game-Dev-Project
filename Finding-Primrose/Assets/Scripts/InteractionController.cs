using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class InteractionController : MonoBehaviour
{
    [Header("Primrose")]
    //public GameObject choiceUI;
    //public CanvasGroup choiceCanvas;
    public PathFollower playerPathFollower;

    //Private variables 
    private ChoiceWaypoint currentWaypoint;
    private bool interactionActive = false;

    public void BeginInteraction(ChoiceWaypoint source)
    {
        if (interactionActive) return;
        interactionActive = true;
        currentWaypoint = source;

        //Freeze Primrose
        if (playerPathFollower != null)
            playerPathFollower.LockMovement();

        if (source.choiceUI != null)
            source.choiceUI.SetActive(true);

        if (CameraController.Instance != null && source.npcFocusPoint != null)
        {
            CameraController.Instance.FocusOn(source.npcFocusPoint);
        }

        StartCoroutine(FadeInChoices(source.choiceCanvas));
    }

    public void ReturnToExploration()
    {
        if (currentWaypoint != null && currentWaypoint.choiceUI != null)
            currentWaypoint.choiceUI.SetActive(false);

        //Unfreeze Primrose
        if (playerPathFollower != null)
        {
            playerPathFollower.UnlockMovement();
        }
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
}
