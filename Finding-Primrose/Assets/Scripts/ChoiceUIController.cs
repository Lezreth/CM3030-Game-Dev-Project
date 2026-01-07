using UnityEngine;

public class ChoiceUIController : MonoBehaviour
{
    public PlayerMove player;
    public GameObject choiceUI;

    public void Return()
    {
        choiceUI.SetActive(false);
        player.canMove = true;
    }

    public void ApproachDockWorker()
    {
        Debug.Log("Approach Dock Worker chosen");
        Return();
    }

    public void ExploreContainers()
    {
        Debug.Log("Explore Containers chosen");
        Return();
    }
}
