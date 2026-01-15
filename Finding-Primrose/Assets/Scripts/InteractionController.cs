using System.Collections;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("References")]
    public PlayerMove player;
    public GameObject choiceUI;
    public Transform npcFocusPoint;
    public CanvasGroup choiceCanvas;
    public ChoiceEffect choiceAEffect;  
    public ChoiceEffect choiceBEffect;

    private bool interactionActive = false;

    public void BeginInteraction()
    {
        if (interactionActive) return;

        interactionActive = true;
        player.canMove = false;

        choiceUI.SetActive(true);
        choiceCanvas.alpha = 0;
        choiceCanvas.interactable = false;
        choiceCanvas.blocksRaycasts = false;

        CameraController.Instance.FocusOn(npcFocusPoint);

        StartCoroutine(WaitForCameraThenFade());
    }

    public void ReturnToExploration()
    {
        choiceUI.SetActive(false);
        player.canMove = true;

        CameraController.Instance.ClearFocus();

        interactionActive = false;
    }

    public void ChooseOption(int optionIndex)
    {
        choiceUI.SetActive(false);

        if (optionIndex == 0 && choiceAEffect != null)
        {
            StatsManager.I.ApplyChoice(choiceAEffect);
            Debug.Log("Choice A made!");
        }
        else if (optionIndex == 1 && choiceBEffect != null)
        {
            StatsManager.I.ApplyChoice(choiceBEffect);
            Debug.Log("Choice B made!");
        }

        ReturnToExploration();
    }

    IEnumerator WaitForCameraThenFade()
    {
        while (!CameraController.Instance.HasArrived())
            yield return null;

        float t = 0f;
        float fadeDuration = 0.4f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            choiceCanvas.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        choiceCanvas.alpha = 1;
        choiceCanvas.interactable = true;
        choiceCanvas.blocksRaycasts = true;
    }

}
