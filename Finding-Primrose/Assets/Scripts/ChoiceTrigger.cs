using UnityEngine;

public class ChoiceTrigger : MonoBehaviour
{
    public GameObject choiceUI;
    public PlayerMove player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.canMove = false;
            choiceUI.SetActive(true);
        }
    }
}

