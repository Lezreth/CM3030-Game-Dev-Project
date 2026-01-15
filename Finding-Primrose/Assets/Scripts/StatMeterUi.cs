///UI controller attached to each stat meter


using UnityEngine;
using UnityEngine.UI;

public class StatMeterUI : MonoBehaviour
{
    [SerializeField] private StatType stat;
    [SerializeField] private Image barFill;

    private void OnEnable()
    {
        if (StatsManager.I != null)
            StatsManager.I.OnStatsChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (StatsManager.I != null)
            StatsManager.I.OnStatsChanged -= Refresh;
    }

    public void Refresh()
    {
        if (StatsManager.I == null || barFill == null) return;

        int value = StatsManager.I.Get(stat);     
        barFill.fillAmount = value / 100f;       
    }
}
