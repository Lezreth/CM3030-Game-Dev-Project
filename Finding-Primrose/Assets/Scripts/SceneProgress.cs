using UnityEngine;
using UnityEngine.UI;

public class SceneProgress : MonoBehaviour
{
    [System.Serializable]
    public class SceneCircle
    {
        public string sceneName;
        public RawImage circle;
    }

    [SerializeField] private SceneCircle[] circles;

    private Color unvisited = new Color(1f, 0.5f, 0.5f, 0.85f);
    private Color visited   = new Color(0.5f, 1f, 0.5f, 0.85f);


        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
           
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, 
                        UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            // Re-subscribe to StatsManager in case it was also just loaded
            if (StatsManager.I != null)
                StatsManager.I.OnStatsChanged += Refresh;
            Refresh();
        }
        void OnEnable()
        {
            subscribed = false;
            if (StatsManager.I != null)
            {
                StatsManager.I.OnStatsChanged -= Refresh;
                StatsManager.I.OnStatsChanged += Refresh;
                subscribed = true;
            }
        }
    void OnDisable()
    {
        if (StatsManager.I != null)
            StatsManager.I.OnStatsChanged -= Refresh;
    }

    void Start()
    {
        Debug.Log($"[Progress] StatsManager found: {StatsManager.I != null}");
        Refresh();
    }

void Update()
{
    
    if (StatsManager.I != null && !subscribed)
    {
        StatsManager.I.OnStatsChanged += Refresh;
        subscribed = true;
        Refresh();
    }
}

private bool subscribed = false;
    void Refresh()
    {
        foreach (var c in circles)
        {
            bool done = StatsManager.I != null &&
                        StatsManager.I.IsSceneCompleted(c.sceneName);
            c.circle.color = done ? visited : unvisited;
        }
    }
}