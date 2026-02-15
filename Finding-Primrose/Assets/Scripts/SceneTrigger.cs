using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load (must match exactly)")]
    public string targetSceneName;
    
    [Tooltip("Message to show player (e.g., 'Travel to Lake Merrit?')")]
    public string confirmationMessage = "Travel to this location?";

    [Header("UI References")]
    public GameObject confirmationPanel;
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    [Header("Transition")]
    public SceneTransitionController transitionController;

    private bool playerInRange = false;

    void Start()
    {
        // Make sure UI is hidden at start
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        // Setup button listeners
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirm);
        
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancel);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowConfirmation();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideConfirmation();
        }
    }

    void ShowConfirmation()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            
            if (messageText != null)
                messageText.text = confirmationMessage;

            // Optionally lock player movement
            PathFollower player = FindObjectOfType<PathFollower>();
            if (player != null)
                player.LockMovement();
        }
    }

    void HideConfirmation()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        // Unlock player movement
        PathFollower player = FindObjectOfType<PathFollower>();
        if (player != null)
            player.UnlockMovement();
    }

    void OnConfirm()
    {
        Debug.Log($"Player confirmed travel to: {targetSceneName}");
        
        HideConfirmation();

        // Start scene transition
        if (transitionController != null)
        {
            transitionController.StartSceneTransition(targetSceneName);
        }
        else
        {
            Debug.LogError("SceneTransitionController not assigned!");
        }
    }

    void OnCancel()
    {
        Debug.Log("Player canceled travel");
        HideConfirmation();
    }

    void OnDrawGizmosSelected()
    {
        // Visualize trigger area
        Gizmos.color = Color.blue;
        BoxCollider boxCol = GetComponent<BoxCollider>();
        if (boxCol != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCol.center, boxCol.size);
        }
    }
}