//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;

//public class SceneTransitionController : MonoBehaviour
//{
//    public Image fadeImage; 

//    [Header("Audio")]
//    public AudioSource audioSource;
//    public AudioClip transitionSound;

//    void Start()
//    {
        
//        if (fadeImage != null)
//        {
//            Color c = fadeImage.color;
//            c.a = 0;
//            fadeImage.color = c;
//        }
//    }

//    public void StartSceneTransition(string sceneName)
//    {
//        Debug.Log("Starting transition to scene: " + sceneName);
//        StartCoroutine(TransitionRoutine(sceneName));
//    }

//    IEnumerator TransitionRoutine(string sceneName)
//    {
//        Debug.Log($"Fading out scene transition for: {sceneName}");
//        Debug.Log("Fade OUT start");

//        if (transitionSound != null && audioSource != null)
//        {
//            audioSource.PlayOneShot(transitionSound);
//        }

//        yield return StartCoroutine(Fade(1f, 0.5f));

//        Debug.Log("Loading scene");
//        SceneManager.LoadScene(sceneName);

//        yield return new WaitForSeconds(0.1f);

//        Debug.Log("Fading in");
//        yield return StartCoroutine(Fade(0f, 0.5f));
//    }

//    IEnumerator Fade(float targetAlpha, float duration)
//    {
//        if (fadeImage == null) yield break;

//        float startAlpha = fadeImage.color.a;
//        float elapsed = 0f;

//        while (elapsed < duration)
//        {
//            elapsed += Time.deltaTime;
//            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            
//            Color c = fadeImage.color;
//            c.a = alpha;
//            fadeImage.color = c;

//            yield return null;
//        }

//        Color finalColor = fadeImage.color;
//        finalColor.a = targetAlpha;
//        fadeImage.color = finalColor;
//    }
//}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public Animator fadeAnimator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip transitionSound;

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