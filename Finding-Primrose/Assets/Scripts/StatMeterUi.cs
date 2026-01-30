///UI controller attached to each stat meter


using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatMeterUI : MonoBehaviour
{
    [SerializeField] private StatType stat;
    [SerializeField] private Image barFill;
    [SerializeField] private TextMeshProUGUI valueText;

    //private void OnEnable()
    //{
    //    if (StatsManager.I != null)
    //        StatsManager.I.OnStatsChanged += Refresh;
    //    Refresh();
    //}
    private void Start()
    {
        if (StatsManager.I != null)
        {
            StatsManager.I.OnStatsChanged += Refresh;
            Refresh();
        }
    }


    private void OnDisable()
    {
        if (StatsManager.I != null)
            StatsManager.I.OnStatsChanged -= Refresh;
    }

    public void Refresh()
    {
        Debug.Log($"Refreshing {stat}");

        if (StatsManager.I == null || barFill == null) return;

        int value = StatsManager.I.Get(stat);     
        barFill.fillAmount = value / 100f;

        if (valueText != null)
            valueText.text = $"{value}%";
    }
}
