using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public Animator fadeAnimator;

    [Header("Transition Audio")]
    public AudioSource audioSource;
    public AudioClip transitionSound;

    [Header("NPC Audio - Wait Before Transitioning")]
    [SerializeField] private List<NPCAudioManager> npcManagers = new List<NPCAudioManager>();

    public void RegisterNPCAudio(NPCAudioManager manager)
    {
        if (manager != null && !npcManagers.Contains(manager))
            npcManagers.Add(manager);
    }

    public void StartSceneTransition(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        
        yield return new WaitWhile(() => npcManagers.Exists(n => n != null && n.IsPlaying()));

        if (transitionSound != null && audioSource != null)
            audioSource.PlayOneShot(transitionSound);

        fadeAnimator.SetTrigger("Out");
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);

        yield return null;
        fadeAnimator.SetTrigger("In");
    }
}