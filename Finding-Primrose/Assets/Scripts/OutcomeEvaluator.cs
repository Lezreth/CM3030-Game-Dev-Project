/// Determines which ending scene to load based on final stats
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutcomeEvaluator : MonoBehaviour
{
    public static string DetermineEndingScene(StatsManager stats)
    {
        int trust = stats.trust;
        int hope = stats.hope;
        int hunger = stats.hunger;
        int energy = stats.energy;

        // Ending 1: Family Finds Her - Best ending
        if (trust >= 70 && hope >= 70 && energy >= 60)
        {
            return "Ending1";  
        }

        // Ending 2: Taken In By Stranger - Worst stats
        if (trust <= 30 || hope <= 30 || energy <= 30)
        {
            return "Ending2"; 
        }

        // Ending 3: Alone But OK - Default/middle range
        //if ((trust >= 65 && trust <= 70) && (hope >= 65 && hope <= 70) && hunger < 50 && (energy >= 50 && energy <= 55))
        //{
        //    return "Ending3";
        //}

        // Ending 4: Alone & Sad - High trust/hunger but low hope/energy
        if (trust >= 70 && hunger >= 70 && (hope < 40 || energy < 50))
        {
            return "Ending4";  
        }

        return "Ending3";
    }
}