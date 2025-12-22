///represents the impact of a one player choice


using System;
using UnityEngine;

[Serializable]
public struct StatDelta
{
    public StatType stat;
    public int delta;
}

[CreateAssetMenu(menuName = "FindingPrimrose/Choice Effect")]
public class ChoiceEffect : ScriptableObject
{
    public string choiceId;          
    public string sceneId;           
    public StatDelta[] deltas;       
}
