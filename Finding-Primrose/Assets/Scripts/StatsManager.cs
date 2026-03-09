

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class StatsManager : MonoBehaviour
{
    public static StatsManager I { get; private set; }

    public bool IsSceneCompleted(string sceneName) => completedScenes.Contains(sceneName);

    [Header("Stats 0-100")]
    [Range(0,100)] public int trust = 50;
    [Range(0,100)] public int hope = 50;
    [Range(0,100)] public int hunger = 70;
    [Range(0,100)] public int energy = 65;

    [Header("Required Scenes")]
    [SerializeField] private string[] requiredScenes;

    [Header("Testing")]
    [SerializeField] private bool enableTestingKeys = true;
    [SerializeField] private bool showDebugHUD = true;

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

    // ── STATS ────────────────────────────────────────────────────

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

    // ── PROGRESS ─────────────────────────────────────────────────

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
            Debug.Log($"[StatsManager] ✓ Scene complete: {currentScene} | {completedScenes.Count}/{requiredScenes.Length}");
        }

        foreach (var d in effect.deltas)
            Add(d.stat, d.delta);

        ClampAll();
        OnStatsChanged?.Invoke();

        // Log full state after every choice
        LogState();

        if (AllRequiredScenesVisited())
            LoadAppropriateEnding();
        else
            Debug.Log($"[StatsManager] {requiredScenes.Length - completedScenes.Count} scene(s) remaining");
    }

    private void LogState()
    {
        Debug.Log(
            $"[StatsManager] Trust:{trust} Hope:{hope} Hunger:{hunger} Energy:{energy} | " +
            $"Scenes: {completedScenes.Count}/{requiredScenes?.Length ?? 0} " +
            $"[{string.Join(", ", completedScenes)}] | " +
            $"→ {OutcomeEvaluator.DetermineEndingScene(this)}"
        );
    }

    // ── DEBUG HUD ───────────────────

void OnGUI()
    {
        if (!showDebugHUD) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 18;
        style.normal.textColor = Color.yellow;

        string completed = string.Join(", ", completedScenes);
        if (string.IsNullOrEmpty(completed)) completed = "none";

        // Show which endings are currently possible
        string e1 = trust >= 70 && hope >= 70 && energy >= 70 ? "✓ Ending1: Family" : "✗ Ending1 (need T≥70 H≥70 E≥70)";
        string e4 = trust >= 70 && hunger >= 70 && hope < 40 && energy < 50 ? "✓ Ending4: Alone&Sad" : "✗ Ending4 (T≥70 Hu≥70 Ho<40 E<50)";
        string e2 = trust <= 30 || hope <= 30 || energy <= 30 ? "✓ Ending2: Stranger" : "✗ Ending2 (need T≤30 or H≤30 or E≤30)";
        string e3 = "✓ Ending3: Alone OK (fallback)";

        GUI.Label(new Rect(10, 10, 600, 280),
            $"Scene: {SceneManager.GetActiveScene().name} | endingTriggered: {endingTriggered}\n" +
            $"Completed ({completedScenes.Count}/{requiredScenes?.Length ?? 0}): {completed}\n" +
            $"All Done: {AllRequiredScenesVisited()}\n" +
            $"Trust:{trust}  Hope:{hope}  Hunger:{hunger}  Energy:{energy}\n\n" +
            $"Possible Endings:\n{e1}\n{e2}\n{e3}\n{e4}",
            style);
    }

    // ── ENDING ───────────────────────────────────────────────────

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

    // ── TESTING ──────────────────────────────────────────────────

    private void Update()
    {
        if (Keyboard.current == null || !enableTestingKeys) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) TestEnding(75, 75, 50, 75);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) TestEnding(25, 25, 50, 25);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) TestEnding(65, 67, 45, 52);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) TestEnding(75, 35, 75, 45);
        if (Keyboard.current.dKey.wasPressedThisFrame)      LogState();
    }

    private void TestEnding(int t, int h, int hu, int e)
    {
        endingTriggered = false;
        trust = t; hope = h; hunger = hu; energy = e;
        ClampAll();
        OnStatsChanged?.Invoke();
        completedScenes.Clear();
        foreach (var s in requiredScenes) completedScenes.Add(s);
        LoadAppropriateEnding();
    }

        public void ResetToDefaults()
    {
        trust = 50;
        hope = 50;
        hunger = 70;
        energy = 65;
        completedScenes.Clear();
        endingTriggered = false;
        ClampAll();
        OnStatsChanged?.Invoke();
    }
}