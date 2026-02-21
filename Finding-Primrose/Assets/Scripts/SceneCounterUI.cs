  using UnityEngine;
using TMPro;


public class SceneCounterUI : MonoBehaviour
{
  



[SerializeField] private TMP_Text hudText;

    void Update()
    {
        if (StatsManager.I == null) return;

        int completed = 0;
        int total = 0;

 
        if (StatsManager.I.AllRequiredScenesVisited())
        {
            completed = 3;
            total = 3;
        }
        else
        {
            total = 3;
     
            completed = CountCompletedScenes();
        }

        hudText.text = $"Scenes: {completed}/{total}\n" +
                       $"Trust:{StatsManager.I.trust} " +
                       $"Hope:{StatsManager.I.hope}\n" +
                       $"Hunger:{StatsManager.I.hunger} " +
                       $"Energy:{StatsManager.I.energy}";
    }

    private int CountCompletedScenes()
    {
       
        return StatsManager.I.CompletedSceneCount();
    }
}