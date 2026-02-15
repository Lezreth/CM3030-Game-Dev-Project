using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public Image fadeImage; // Drag your black panel Image here

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
            
            if (fadeImage != null)
            {
                DontDestroyOnLoad(fadeImage.transform.root.gameObject); // Persist the canvas
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start clear
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;
        }
    }

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
<<<<<<< HEAD
        Debug.Log("Fading out");
=======
        Debug.Log($"Fading out scene transition for: {sceneName}");
        Debug.Log("Fade OUT start");

        if (transitionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        fadeAnimator.SetTrigger("Out");
>>>>>>> 1788fc8363705f7e600342952f889eb579dd7834

        if (transitionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        // Fade to black
        yield return StartCoroutine(Fade(1f, 0.5f));

        Debug.Log("Loading scene");
        SceneManager.LoadScene(sceneName);

        yield return new WaitForSeconds(0.1f);

        // Fade to clear
        Debug.Log("Fading in");
        yield return StartCoroutine(Fade(0f, 0.5f));
    }

    IEnumerator Fade(float targetAlpha, float duration)
    {
        if (fadeImage == null) yield break;

        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;

            yield return null;
        }

        // Ensure we hit target
        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;
    }
}