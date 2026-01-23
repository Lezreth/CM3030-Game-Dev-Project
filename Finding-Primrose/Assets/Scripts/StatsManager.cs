///owns current game state for all stats


using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager I { get; private set; }

    [Header("Stats 0-100")]
    [Range(0,100)] public int trust = 40;
    [Range(0,100)] public int hope = 45;
    [Range(0,100)] public int hunger = 50;
    [Range(0,100)] public int energy = 65;

    [Header("Progress")]
    [SerializeField] private string[] requiredSceneIds; 

    private readonly HashSet<string> visitedScenes = new();

    public event Action OnStatsChanged;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
       
        ClampAll();
    }

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

        // mark scene visited
        if (!string.IsNullOrEmpty(effect.sceneId))
            visitedScenes.Add(effect.sceneId);

        // apply deltas
        foreach (var d in effect.deltas)
            Add(d.stat, d.delta);

        ClampAll();
        OnStatsChanged?.Invoke();
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
        foreach (var id in requiredSceneIds)
            if (!visitedScenes.Contains(id)) return false;
        return true;
    }
}
