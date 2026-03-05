using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager I { get; private set; }

    private static readonly int SaturationID = Shader.PropertyToID("_Saturation");


    private static readonly HashSet<string> endingScenes = new()
    {
        "Ending1", "Ending2", "Ending3", "Ending4" 
    };

    [Header("Music")]
    public AudioClip musicHappy;    // Ending 1: Family Finds Her
    public AudioClip musicNeutral;  // Ending 3: Alone But OK
    public AudioClip musicBad;      // Ending 4: Alone & Sad
    public AudioClip musicEnergetic;// Ending 2: Taken In By Stranger
    public AudioClip musicStart;    // Start music


    [Header("Saturation")]
    [SerializeField] private float saturationHappy = 1f;
    [SerializeField] private float saturationNeutral = 0.6f;
    [SerializeField] private float saturationBad = 0f;
    [SerializeField] private float saturationEnergetic = .5f;
    [SerializeField] private float saturationStart = 0.8f;
    [Header("Settings")]
    [SerializeField] private float crossfadeDuration = 2f;
    [SerializeField] private float masterVolume = 1f;

    private AudioSource activeSouce => isPlayingA ? sourceA : sourceB;
    private AudioSource inactiveSource => isPlayingA ? sourceB : sourceA;
    private bool isPlayingA = true;
    private Coroutine currentFade;
    private AudioClip currentClip;


    private AudioSource sourceA;
    private AudioSource sourceB;
   private void Awake()
    {
       

    Debug.Log($"[MusicManager] Awake called. Already exists: {I != null}");
    if (I != null && I != this) { Destroy(gameObject); return; }
    I = this;
    DontDestroyOnLoad(gameObject);
    sourceA = gameObject.AddComponent<AudioSource>();
    sourceB = gameObject.AddComponent<AudioSource>();
    sourceA.loop = true;
    sourceB.loop = true;
    sourceA.playOnAwake = false;
    sourceB.playOnAwake = false;
    Shader.SetGlobalFloat(SaturationID, saturationStart);

    }

    private void Start()
    {
     
        PlayImmediate(musicStart);

        
        if (StatsManager.I != null)
            StatsManager.I.OnStatsChanged += OnStatsChanged;
    }

   
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
                                UnityEngine.SceneManagement.LoadSceneMode mode)
    {
            Debug.Log($"[MusicManager] Scene loaded: {scene.name}");
            Debug.Log($"[MusicManager] sourceA null: {sourceA == null} | sourceB null: {sourceB == null}");
            Debug.Log($"[MusicManager] currentClip: {(currentClip != null ? currentClip.name : "NULL")}");
            Debug.Log($"[MusicManager] isPlaying: {activeSouce?.isPlaying}");

        if (endingScenes.Contains(scene.name))
        {
            if (StatsManager.I != null)
                StatsManager.I.ResetToDefaults();

            PlayImmediate(musicStart);
            Shader.SetGlobalFloat(SaturationID, saturationStart);
            return;
        }

        if (StatsManager.I != null)
        {
            StatsManager.I.OnStatsChanged -= OnStatsChanged;
            StatsManager.I.OnStatsChanged += OnStatsChanged;
        }

        if (!activeSouce.isPlaying && currentClip != null)
        {
            activeSouce.clip = currentClip;
            activeSouce.volume = masterVolume;
            activeSouce.loop = true;
            activeSouce.Play();
        }
    }

    private void OnStatsChanged()
    {
        AudioClip target = DetermineTargetClip();
        if (target != currentClip)
            CrossfadeTo(target);
    }

    private AudioClip DetermineTargetClip()
    {
        // Mirror OutcomeEvaluator logic exactly
        int trust  = StatsManager.I.trust;
        int hope   = StatsManager.I.hope;
        int energy = StatsManager.I.energy;
        int hunger = StatsManager.I.hunger;

        // Ending 1: Family Finds Her → Happy
        if (hope >= 70 && trust >= 70 && energy >= 60)
            return musicHappy;

        // Ending 4: Alone & Sad → Bad
        if (trust >= 70 && hope < 50 && energy < 45)
            return musicBad;

        // Ending 2: Taken In By Stranger → Energetic
        if (trust <= 35 && hope <= 45)
            return musicEnergetic;

        // Ending 3: Alone But OK → Neutral
        return musicNeutral;
    }

    private void CrossfadeTo(AudioClip clip)
    {
        if (clip == null) return;
        currentClip = clip;

        // Set saturation based on its clip
        if (clip == musicHappy)        Shader.SetGlobalFloat(SaturationID, saturationHappy);
        else if (clip == musicBad)     Shader.SetGlobalFloat(SaturationID, saturationBad);
        else if (clip == musicEnergetic) Shader.SetGlobalFloat(SaturationID, saturationEnergetic);
        else if (clip == musicNeutral) Shader.SetGlobalFloat(SaturationID, saturationNeutral);
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(CrossfadeRoutine(clip));
    }

    private IEnumerator CrossfadeRoutine(AudioClip clip)
    {
        // Set up incoming track on inactive source
        inactiveSource.clip = clip;
        inactiveSource.volume = 0f;
        inactiveSource.loop = true;
        inactiveSource.Play();

        float t = 0f;
        AudioSource fadeOut = activeSouce;
        AudioSource fadeIn  = inactiveSource;
        float startVolume   = fadeOut.volume;

        while (t < crossfadeDuration)
        {
            t += Time.deltaTime;
            float ratio = t / crossfadeDuration;
            fadeOut.volume = Mathf.Lerp(startVolume, 0f, ratio) * masterVolume;
            fadeIn.volume  = Mathf.Lerp(0f, masterVolume, ratio);
            yield return null;
        }

        fadeOut.Stop();
        fadeOut.clip = null;
        isPlayingA = !isPlayingA;
    }


    // Call this from ending scenes to fade out music
    public void FadeOut(float duration = 2f)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeOutRoutine(duration));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float startVolume = activeSouce.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            activeSouce.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        activeSouce.Stop();
    }

        private void PlayImmediate(AudioClip clip)
    {
        if (clip == null) return;
        currentClip = clip;
        Shader.SetGlobalFloat(SaturationID, saturationStart);
        activeSouce.clip = clip;
        activeSouce.volume = masterVolume;
        activeSouce.loop = true;
        activeSouce.Play();
    }

        private void OnDestroy()
    {
        Debug.Log("[MusicManager] I AM BEING DESTROYED");
    }
}