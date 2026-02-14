using System.Collections;
using UnityEngine;

public class NPCAudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioStep
    {
        public AudioClip clip;
        [Tooltip("Seconds to wait AFTER this clip finishes before the next one")]
        public float delayAfter = 0f;
    }

    [System.Serializable]
    public class AudioSequence
    {
        public AudioStep[] steps;
    }

    [Header("Audio Source")]
    public AudioSource source;

    [Header("Approach Sequence (plays when player arrives, BEFORE choices)")]
    public AudioSequence approachSequence;

    [Header("Choice A Sequence (plays AFTER choice A selected)")]
    public AudioSequence choiceA;

    [Header("Choice B Sequence (plays AFTER choice B selected)")]
    public AudioSequence choiceB;

    private Coroutine currentPlayback;

    // Called when player first approaches NPC (before choices shown)
    public void PlayApproachSequence()
    {
        PlaySequence(approachSequence);
    }

    // Called when Choice A is selected
    public void PlayChoiceA()
    {
        PlaySequence(choiceA);
    }

    // Called when Choice B is selected
    public void PlayChoiceB()
    {
        PlaySequence(choiceB);
    }

    // Generic play method
    public void PlaySequence(AudioSequence sequence)
    {
        if (sequence == null || sequence.steps == null || sequence.steps.Length == 0)
            return;

        if (currentPlayback != null)
            StopCoroutine(currentPlayback);

        currentPlayback = StartCoroutine(PlayRoutine(sequence));
    }

    private IEnumerator PlayRoutine(AudioSequence sequence)
    {
        foreach (var step in sequence.steps)
        {
            if (step.clip == null)
                continue;

            source.clip = step.clip;
            source.Play();

            // Wait until the clip finishes
            yield return new WaitForSeconds(step.clip.length);

            // Optional extra pause
            if (step.delayAfter > 0f)
                yield return new WaitForSeconds(step.delayAfter);
        }

        currentPlayback = null;
    }

    public float GetApproachSequenceDuration()
    {
        if (approachSequence == null || approachSequence.steps == null)
            return 0f;

        float totalDuration = 0f;
        foreach (var step in approachSequence.steps)
        {
            if (step.clip != null)
            {
                totalDuration += step.clip.length;
                totalDuration += step.delayAfter;
            }
        }
        return totalDuration;
    }
}