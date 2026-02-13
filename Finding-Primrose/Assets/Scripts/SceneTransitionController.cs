using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public Animator fadeAnimator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip transitionSound;

    private static SceneTransitionController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);  
        }
        else
        {
            Destroy(gameObject);  
        }
    }

    public void StartSceneTransition(string sceneName)
    {
        Debug.Log("Starting transition to scene: " + sceneName);
        StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        Debug.Log($"Fading out scene transition for: {sceneName}");
        Debug.Log("Fade OUT start");

        if (transitionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        fadeAnimator.SetTrigger("Out");

        yield return new WaitForSeconds(1f);

        Debug.Log("Loading scene");
        SceneManager.LoadScene(sceneName);

        yield return null; 

        fadeAnimator.SetTrigger("In");
    }
}

