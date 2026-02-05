using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public Animator fadeAnimator;

    public void StartSceneTransition(string sceneName)
    {
        Debug.Log("Starting transition to scene: " + sceneName);
        StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        Debug.Log($"Fading out scene transition for: {sceneName}");
        Debug.Log("Fade OUT start");
        fadeAnimator.SetTrigger("Out");

        yield return new WaitForSeconds(1f);

        Debug.Log("Loading scene");
        SceneManager.LoadScene(sceneName);

        yield return null; 

        fadeAnimator.SetTrigger("In");
    }
}

