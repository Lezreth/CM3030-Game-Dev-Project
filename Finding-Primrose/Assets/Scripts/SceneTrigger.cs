using UnityEngine;
using UnityEngine.UI;

public class EdgeSceneTrigger : MonoBehaviour
{
    [Header("Interaction System")]
    public InteractionController interaction;
    public GameObject uiPanel;
    public CanvasGroup uiCanvas;
    public Transform focusPoint;
    
    [Header("UI Buttons")]
    public Button confirmButton;  
    public Button cancelButton;  
    
    [Header("Scene")]
    public string sceneToLoad;
    public SceneTransitionController transitionController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[EdgeTrigger] PLAYER ENTERED! Showing UI for {sceneToLoad}");
            
            // Hook up buttons to THIS trigger
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveAllListeners();  // Clear old assignments
                confirmButton.onClick.AddListener(ConfirmTransition);  // Assign to THIS trigger
                Debug.Log("[EdgeTrigger] Confirm button hooked up!");
            }
            
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(CancelTransition);
            }
            
            // Show UI
            interaction.BeginInteraction(uiPanel, uiCanvas, focusPoint);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[EdgeTrigger] Player exited - returning to exploration");
            interaction.ReturnToExploration();
        }
    }

    public void ConfirmTransition()
    {
       
        Debug.Log($"[EdgeTrigger] ConfirmTransition() CALLED!");
        Debug.Log($"[EdgeTrigger] Scene to load: {sceneToLoad}");
        
        interaction.ReturnToExploration();
        transitionController.StartSceneTransition(sceneToLoad);
    }

    public void CancelTransition()
    {
        Debug.Log("[EdgeTrigger] Cancelled");
        interaction.ReturnToExploration();
    }
}