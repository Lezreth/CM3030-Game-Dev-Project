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

    [Header("Proximity Detection")]
    public float triggerDistance = 3.5f;
    public string playerTag = "Player";
    
    private bool hasPlayedApproach = false;
    private Transform playerTransform;
    private Coroutine currentPlayback;
    
    // debug logging
    private float lastDebugTime = 0f;
    private float debugInterval = 0.5f; 

    void Start()
    {
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log($"[NPCAudio] Found player: {player.name}");
        }
        else
        {
            Debug.LogError($"[NPCAudio] Could not find player with tag '{playerTag}'!");
        }
        
        // Check audio source
        if (source == null)
        {
            Debug.LogError("[NPCAudio] Audio Source is NULL!");
        }
        else
        {
            Debug.Log($"[NPCAudio] Audio Source found: {source.name}");
        }
        
        // Check approach sequence
        if (approachSequence == null || approachSequence.steps == null || approachSequence.steps.Length == 0)
        {
            Debug.LogWarning("[NPCAudio] Approach sequence is empty!");
        }
        else
        {
            Debug.Log($"[NPCAudio] Approach sequence has {approachSequence.steps.Length} steps");
        }
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        
        // Print distance periodically
        if (Time.time - lastDebugTime >= debugInterval)
        {
            //Debug.Log($"[NPCAudio] Distance to player: {distance:F2}m (trigger at {triggerDistance}m)");
            lastDebugTime = Time.time;
        }
        
        if (!hasPlayedApproach && distance <= triggerDistance)
        {
            // Debug.Log("════════════════════════════════════════");
            // Debug.Log($"[NPCAudio] TRIGGERED! Player within {triggerDistance}m!");
            // Debug.Log($"[NPCAudio] Current distance: {distance:F2}m");
            // Debug.Log($"[NPCAudio] Playing approach sequence NOW");
            // Debug.Log("════════════════════════════════════════");
            
            hasPlayedApproach = true;
            PlayApproachSequence();
        }
    }

    public void PlayApproachSequence()
    {
       
        PlaySequence(approachSequence);
    }

    public void PlayChoiceA()
    {
       
        PlaySequence(choiceA);
    }

    public void PlayChoiceB()
    {
       
        PlaySequence(choiceB);
    }

    public void PlaySequence(AudioSequence sequence)
    {
        
        
        if (sequence == null || sequence.steps == null || sequence.steps.Length == 0)
        {
           
            return;
        }

        

        if (currentPlayback != null)
        {
            
            StopCoroutine(currentPlayback);
        }

        currentPlayback = StartCoroutine(PlayRoutine(sequence));
    }

    private IEnumerator PlayRoutine(AudioSequence sequence)
    {
        
        
        int stepIndex = 0;
        foreach (var step in sequence.steps)
        {
            stepIndex++;
            
            if (step.clip == null)
            {
                Debug.LogWarning($"[NPCAudio] Step {stepIndex} has no clip - skipping");
                continue;
            }

            Debug.Log($"[NPCAudio] ▶ Playing step {stepIndex}/{sequence.steps.Length}: {step.clip.name} (length: {step.clip.length:F2}s)");
            
            source.clip = step.clip;
            source.Play();
            
            Debug.Log($"[NPCAudio] Audio source playing? {source.isPlaying}");

            // Wait until the clip finishes
            yield return new WaitForSeconds(step.clip.length);
            
            Debug.Log($"[NPCAudio] ✓ Step {stepIndex} finished");

            // Optional extra pause
            if (step.delayAfter > 0f)
            {
                Debug.Log($"[NPCAudio] ⏸ Waiting {step.delayAfter:F2}s delay...");
                yield return new WaitForSeconds(step.delayAfter);
            }
        }

        Debug.Log("[NPCAudio] ✓ SEQUENCE COMPLETE");
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

    public void ResetApproachTrigger()
    {
        Debug.Log("[NPCAudio] Approach trigger reset");
        hasPlayedApproach = false;
    }
}