using UnityEngine;
using TMPro;

public class SceneCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hudText;

    void Update()
    {
        if (StatsManager.I == null) return;

        int completed = StatsManager.I.CompletedSceneCount();
        bool allDone = StatsManager.I.AllRequiredScenesVisited();

        hudText.text = $"Scenes: {completed} | All Done: {allDone}\n" +
                       $"Trust:{StatsManager.I.trust} " +
                       $"Hope:{StatsManager.I.hope}\n" +
                       $"Hunger:{StatsManager.I.hunger} " +
                       $"Energy:{StatsManager.I.energy}\n" +
                       $"→ {OutcomeEvaluator.DetermineEndingScene(StatsManager.I)}";
    }
}