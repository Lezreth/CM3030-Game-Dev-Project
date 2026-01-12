using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("References")]
    public PlayerMove player;
    public GameObject choiceUI;
    public Transform npcFocusPoint;

    private bool interactionActive = false;

    public void BeginInteraction()
    {
        if (interactionActive) return;

        interactionActive = true;
        player.canMove = false;

        // Later:
        CameraController.Instance.FocusOn(npcFocusPoint);

        choiceUI.SetActive(true);
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

        // Later:
        // Play animation
        // Modify stats
        // Load scene or reposition player

        Debug.Log("Choice made: " + optionIndex);
    }
}
