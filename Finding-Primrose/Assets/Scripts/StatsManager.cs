

///owns current game state for all stats


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.InputSystem; 

public class StatsManager : MonoBehaviour
{
    public static StatsManager I { get; private set; }

    

    [Header("Stats 0-100")]
    [Range(0,100)] public int trust = 40;
    [Range(0,100)] public int hope = 45;
    [Range(0,100)] public int hunger = 50;
    [Range(0,100)] public int energy = 65;

    [Header("Progress - Required Scenes")]
    [SerializeField] private string[] requiredScenes;

 
    private string sceneToLoad;

    public event Action OnStatsChanged;

    

    private readonly HashSet<string> completedScenes = new();

    

    [Header("Testing")]
    [SerializeField] private bool enableTestingKeys = true;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
       
        completedScenes.Clear(); 
        ClampAll();
       
        
        Debug.Log("=== StatsManager Initialized ===");
        Debug.Log($"Testing Keys Enabled: {enableTestingKeys}");
    }
private void Update()
{
    // Check if keyboard exists (new Input System)
    if (Keyboard.current == null)
    {
        Debug.LogWarning("Keyboard not detected!");
        return;
    }

    if (!enableTestingKeys)
    {
        // Debug why keys aren't working
        if (Keyboard.current.digit1Key.wasPressedThisFrame || 
            Keyboard.current.digit2Key.wasPressedThisFrame || 
            Keyboard.current.digit3Key.wasPressedThisFrame || 
            Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.LogWarning("Testing keys are DISABLED. Enable them in StatsManager Inspector!");
        }
        return;
    }

    // Debug key presses
    if (Keyboard.current.digit1Key.wasPressedThisFrame)
    {
        Debug.Log("KEY PRESSED: 1");
        TestEnding1();
    }
    if (Keyboard.current.digit2Key.wasPressedThisFrame)
    {
        Debug.Log("KEY PRESSED: 2");
        TestEnding2();
    }
    if (Keyboard.current.digit3Key.wasPressedThisFrame)
    {
        Debug.Log("KEY PRESSED: 3");
        TestEnding3();
    }
    if (Keyboard.current.digit4Key.wasPressedThisFrame)
    {
        Debug.Log("KEY PRESSED: 4");
        TestEnding4();
    }

    // Debug helper - press D to check current state
    if (Keyboard.current.dKey.wasPressedThisFrame)
    {
        DebugCurrentState();
    }
}

    private void DebugCurrentState()
    {
        Debug.Log("=== CURRENT STATE ===");
        Debug.Log($"Stats Manager Exists: {I != null}");
        Debug.Log($"Testing Keys Enabled: {enableTestingKeys}");
        Debug.Log($"Current Stats - Trust:{trust} Hope:{hope} Hunger:{hunger} Energy:{energy}");
        Debug.Log($"Required Scenes: {requiredScenes.Length}");
        for (int i = 0; i < requiredScenes.Length; i++)
        {
            Debug.Log($"  [{i}] {requiredScenes[i]}");
        }
        Debug.Log($"Completed Scenes: {completedScenes.Count}");
        foreach (var scene in completedScenes)
        {
            Debug.Log($"  ✓ {scene}");
        }
        Debug.Log($"Current Scene: {SceneManager.GetActiveScene().name}");
        
        
        if (requiredScenes.Length > 0)
        {
            string wouldTrigger = OutcomeEvaluator.DetermineEndingScene(this);
            Debug.Log($"Would trigger ending: {wouldTrigger}");
        }
        Debug.Log("===================");
    }

    #region Testing Functions
    
    private void TestEnding1()
    {
        Debug.Log("<color=cyan>=== TESTING ENDING 1: Family Finds Her ===</color>");
        try
        {
            SetTestStats(75, 75, 50, 75);
            MarkAllScenesComplete();
            LoadAppropriateEnding();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in TestEnding1: {e.Message}\n{e.StackTrace}");
        }
    }

    private void TestEnding2()
    {
        Debug.Log("<color=green>=== TESTING ENDING 2: Taken In By Stranger ===</color>");
        try
        {
            SetTestStats(25, 25, 50, 25);
            MarkAllScenesComplete();
            LoadAppropriateEnding();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in TestEnding2: {e.Message}\n{e.StackTrace}");
        }
    }

    private void TestEnding3()
    {
        Debug.Log("<color=yellow>=== TESTING ENDING 3: Alone But OK ===</color>");
        try
        {
            SetTestStats(45, 55, 50, 55);
            MarkAllScenesComplete();
            LoadAppropriateEnding();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in TestEnding3: {e.Message}\n{e.StackTrace}");
        }
    }

    private void TestEnding4()
    {
        Debug.Log("<color=blue>=== TESTING ENDING 4: Alone & Sad ===</color>");
        try
        {
            SetTestStats(75, 35, 75, 45);
            MarkAllScenesComplete();
            LoadAppropriateEnding();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in TestEnding4: {e.Message}\n{e.StackTrace}");
        }
    }

    private void SetTestStats(int trustVal, int hopeVal, int hungerVal, int energyVal)
    {
        trust = trustVal;
        hope = hopeVal;
        hunger = hungerVal;
        energy = energyVal;
        
        ClampAll();
        OnStatsChanged?.Invoke();
        
        Debug.Log($"✓ Stats set → Trust:{trust} Hope:{hope} Hunger:{hunger} Energy:{energy}");
    }

    private void MarkAllScenesComplete()
    {
        completedScenes.Clear();
        
        if (requiredScenes == null || requiredScenes.Length == 0)
        {
            Debug.LogWarning("No required scenes defined in StatsManager!");
            return;
        }
        
        foreach (var sceneName in requiredScenes)
        {
            completedScenes.Add(sceneName);
            Debug.Log($"  + Marked '{sceneName}' as complete");
        }
        Debug.Log($"✓ Total scenes marked complete: {requiredScenes.Length}");
    }

    #endregion

    public int Get(StatType stat) => stat switch
    {
        StatType.Trust => trust,
        StatType.Hope => hope,
        StatType.Hunger => hunger,
        StatType.Energy => energy,
        _ => 0
    };

    public void ApplyChoice(ChoiceEffect effect)
    {
        if (effect == null) return;

        string currentScene = SceneManager.GetActiveScene().name;
        if (!completedScenes.Contains(currentScene))
        {
            completedScenes.Add(currentScene);
            Debug.Log($"✓ Completed: {currentScene} | Progress: {completedScenes.Count}/{requiredScenes.Length}");
        }

        foreach (var d in effect.deltas)
            Add(d.stat, d.delta);

        ClampAll();
        OnStatsChanged?.Invoke();

        if (AllRequiredScenesVisited())
        {
            Debug.Log("All required scenes visited! Triggering ending...");
            LoadAppropriateEnding();
        }
    }
private void LoadAppropriateEnding()
{
    Debug.Log("=== LoadAppropriateEnding Called ===");
    
    string endingScene = OutcomeEvaluator.DetermineEndingScene(this);
    Debug.Log($"<color=orange>Determined ending: {endingScene}</color>");
    
    // Check if scene exists in build settings
    bool sceneExists = false;
    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        if (sceneName == endingScene)
        {
            sceneExists = true;
            break;
        }
    }
    
    if (!sceneExists)
    {
        Debug.LogError($"Scene '{endingScene}' NOT found in Build Settings! Add it via File → Build Settings");
        return;
    }
    
    Debug.Log($"Scene '{endingScene}' found in build settings. Loading in 1.5s...");
    DontDestroyOnLoad(gameObject);
    
    sceneToLoad = endingScene;
    Invoke(nameof(LoadEnding), 1.5f);
}

private void LoadEnding()
{
    Debug.Log($"<color=cyan>NOW LOADING: {sceneToLoad}</color>");
    SceneManager.LoadScene(sceneToLoad);
}
    public void Add(StatType stat, int delta)
    {
        switch (stat)
        {
            case StatType.Trust:  trust += delta;  break;
            case StatType.Hope:   hope += delta;   break;
            case StatType.Hunger: hunger += delta; break;
            case StatType.Energy: energy += delta; break;
        }
    }

    private void ClampAll()
    {
        trust  = Mathf.Clamp(trust, 0, 100);
        hope   = Mathf.Clamp(hope, 0, 100);
        hunger = Mathf.Clamp(hunger, 0, 100);
        energy = Mathf.Clamp(energy, 0, 100);
    }

    public bool AllRequiredScenesVisited()
    {
        foreach (var sceneName in requiredScenes)
            if (!completedScenes.Contains(sceneName)) return false;
        return true;
    }
}