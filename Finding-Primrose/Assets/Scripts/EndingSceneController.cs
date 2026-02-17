/// NEW FILE - Create this
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EndingSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Button restartButton;
    [SerializeField] private CanvasGroup uiCanvasGroup;
    
    [Header("This Ending's Content")]
    [SerializeField] private string endingTitle = "ENDING 1: FAMILY FINDS HER";
    [TextArea(3, 10)]
    [SerializeField] private string endingStory = "A car pulls up → someone calls her name...";
    
    [Header("Settings")]
    [SerializeField] private float fadeInDelay = 2f;
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private string firstSceneName = "Harbor";
    
    private void Start()
    {
        if (titleText != null) titleText.text = endingTitle;
        if (storyText != null) storyText.text = endingStory;
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false);
        }
        
        StartCoroutine(FadeInSequence());
    }
    
    private IEnumerator FadeInSequence()
    {
        if (uiCanvasGroup != null)
            uiCanvasGroup.alpha = 0f;
        
        yield return new WaitForSeconds(fadeInDelay);
        
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            if (uiCanvasGroup != null)
                uiCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        
        if (uiCanvasGroup != null)
            uiCanvasGroup.alpha = 1f;
        
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);
    }
    
    public void RestartGame()
    {

        Debug.Log("Restarting game from ending scene...");
        if (StatsManager.I != null)
        {
            Destroy(StatsManager.I.gameObject);
        }
        
        SceneManager.LoadScene(firstSceneName);
    }
}