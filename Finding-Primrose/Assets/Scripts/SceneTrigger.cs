using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [Header("Scene to Load")]
    [Tooltip("Exact name of the scene (e.g., 'FoodTruck' or 'LakeMerrit')")]
    public string targetSceneName;

    [Header("References")]
    public SceneTransitionController transitionController;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered trigger for scene: {targetSceneName}");
            
            // Start the scene transition
            if (transitionController != null)
            {
                transitionController.StartSceneTransition(targetSceneName);
            }
            else
            {
                Debug.LogError("SceneTransitionController not assigned!");
            }
        }
    }
}