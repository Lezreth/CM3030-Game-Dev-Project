using UnityEngine;

public class ChoiceTrigger : MonoBehaviour
{
    public GameObject choiceUI;
    public PlayerMove player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.canMove = false;
            choiceUI.SetActive(true);
        }
    }
}

