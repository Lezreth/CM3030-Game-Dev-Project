using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class StatsManager : MonoBehaviour
{
    public static StatsManager I { get; private set; }

    [Header("Stats 0-100")]
    [Range(0,100)] public int trust = 50;
    [Range(0,100)] public int hope = 50;
    [Range(0,100)] public int hunger = 70;
    [Range(0,100)] public int energy = 40;

    [Header("Required Scenes")]
    [SerializeField] private string[] requiredScenes;

    [Header("Testing")]
    [SerializeField] private bool enableTestingKeys = true;

    public event Action OnStatsChanged;

    private readonly HashSet<string> completedScenes = new();
    private bool endingTriggered = false;


    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        ClampAll();
    }

 
    // STATS


    public int Get(StatType stat) => stat switch
    {
        StatType.Trust  => trust,
        StatType.Hope   => hope,
        StatType.Hunger => hunger,
        StatType.Energy => energy,
        _ => 0
    };

    public void Add(StatType stat, int delta)
    {
        switch (stat)
        {
            case StatType.Trust:  trust  += delta; break;
            case StatType.Hope:   hope   += delta; break;
            case StatType.Hunger: hunger += delta; break;
            case StatType.Energy: energy += delta; break;
        }
    }

    private void ClampAll()
    {
        trust  = Mathf.Clamp(trust,  0, 100);
        hope   = Mathf.Clamp(hope,   0, 100);
        hunger = Mathf.Clamp(hunger, 0, 100);
        energy = Mathf.Clamp(energy, 0, 100);
    }

   
    // PROGRESS
   

    public int CompletedSceneCount() => completedScenes.Count;

    public bool AllRequiredScenesVisited()
    {
        if (requiredScenes == null || requiredScenes.Length == 0) return false;
        foreach (var scene in requiredScenes)
            if (!completedScenes.Contains(scene)) return false;
        return true;
    }

    public void ApplyChoice(ChoiceEffect effect)
    {
        if (effect == null || endingTriggered) return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (!completedScenes.Contains(currentScene))
        {
            completedScenes.Add(currentScene);
            Debug.Log($"[StatsManager] ✓ {currentScene} complete | {completedScenes.Count}/{requiredScenes.Length}");
        }

        foreach (var d in effect.deltas)
            Add(d.stat, d.delta);

        ClampAll();
        OnStatsChanged?.Invoke();

        if (AllRequiredScenesVisited())
            LoadAppropriateEnding();
        else
            Debug.Log($"[StatsManager] {requiredScenes.Length - completedScenes.Count} scene(s) remaining");
    }

    
    // ENDING
   

    private void LoadAppropriateEnding()
    {
        if (endingTriggered) return;
        endingTriggered = true;

        string endingScene = OutcomeEvaluator.DetermineEndingScene(this);

        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(path) == endingScene)
            {
                sceneExists = true;
                break;
            }
        }

        if (!sceneExists)
        {
            Debug.LogError($"[StatsManager] Ending scene '{endingScene}' not in Build Settings!");
            return;
        }

        SceneManager.LoadScene(endingScene);
    }

  
    // // TESTING
    

    // private void Update()
    // {
    //     if (Keyboard.current == null || !enableTestingKeys) return;

    //     if (Keyboard.current.digit1Key.wasPressedThisFrame) TestEnding(75, 75, 50, 75);
    //     if (Keyboard.current.digit2Key.wasPressedThisFrame) TestEnding(25, 25, 50, 25);
    //     if (Keyboard.current.digit3Key.wasPressedThisFrame) TestEnding(65, 67, 45, 52);
    //     if (Keyboard.current.digit4Key.wasPressedThisFrame) TestEnding(75, 35, 75, 45);
    //     if (Keyboard.current.dKey.wasPressedThisFrame)      DebugCurrentState();
    // }

    // private void TestEnding(int t, int h, int hu, int e)
    // {
    //     endingTriggered = false;
    //     trust = t; hope = h; hunger = hu; energy = e;
    //     ClampAll();
    //     OnStatsChanged?.Invoke();
    //     completedScenes.Clear();
    //     foreach (var s in requiredScenes) completedScenes.Add(s);
    //     LoadAppropriateEnding();
    // }

    // private void DebugCurrentState()
    // {
    //     Debug.Log($"[StatsManager] Trust:{trust} Hope:{hope} Hunger:{hunger} Energy:{energy}");
    //     Debug.Log($"[StatsManager] Scenes: {completedScenes.Count}/{requiredScenes.Length}");
    //     Debug.Log($"[StatsManager] Would load: {OutcomeEvaluator.DetermineEndingScene(this)}");
    // }
}