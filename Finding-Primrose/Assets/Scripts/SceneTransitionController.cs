using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public Animator fadeAnimator;

    public void StartSceneTransition(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        fadeAnimator.SetTrigger("Out");

        yield return new WaitForSeconds(1f); 

        SceneManager.LoadScene(sceneName);

        yield return null; 

        fadeAnimator.SetTrigger("In");
    }
}

