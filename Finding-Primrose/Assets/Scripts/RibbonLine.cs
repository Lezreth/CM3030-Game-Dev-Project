using UnityEngine;

public class RibbonLine : MonoBehaviour
{
    public LineRenderer line;
    public Transform start;
    public Transform control;
    public Transform end;

    public int samples = 40;
    public float duration = 1f;
    public float maxWidth = 0.1f;

    public AnimationCurve revealCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public float wobble = 0.1f;
    public float wobbleSpeed = 1.5f;

    Vector3[] points;
    float t;
    bool playing;

    void Awake()
    {
   if (!line) line = GetComponent<LineRenderer>();
    points = new Vector3[samples];

    line.enabled = true;
    line.positionCount = samples;
    line.useWorldSpace = true; 
    
    }

    void Start()
{
      // 🔍 DEBUGGING
    Debug.Log($"=== RibbonLine Debug ===");
    Debug.Log($"Start: {(start ? start.name + " at " + start.position : "NULL")}");
    Debug.Log($"Control: {(control ? control.name + " at " + control.position : "NULL")}");
    Debug.Log($"End: {(end ? end.name + " at " + end.position : "NULL")}");
    Debug.Log($"LineRenderer: {(line ? "Found" : "NULL")}");
    
    // ✅ Force LineRenderer settings
    line.useWorldSpace = true;
    line.alignment = LineAlignment.View; // or LineAlignment.TransformZ
    line.textureMode = LineTextureMode.Stretch;
    
    // Auto-play for testing
    Play();
}

    public void Play()
    {
        t = 0;
        playing = true;
        line.enabled = true;
        line.positionCount = 0;
    }

    void Update()
    {

        if (!playing) return;

        // Debug visualization
        Debug.DrawLine(start.position, control.position, Color.green);
        Debug.DrawLine(control.position, end.position, Color.green);
    
       


        float r = Mathf.Clamp01(revealCurve.Evaluate(t));

        Vector3 ctrl = control.position;
        ctrl += new Vector3(
            Mathf.PerlinNoise(Time.time * wobbleSpeed, 0) - 0.5f,
            Mathf.PerlinNoise(0, Time.time * wobbleSpeed) - 0.5f,
            0
        ) * wobble;

        for (int i = 0; i < samples; i++)
        {
            float u = i / (samples - 1f);
            points[i] = Bezier(start.position, ctrl, end.position, u);
        }

        int visible = Mathf.Clamp(Mathf.CeilToInt(r * samples), 2, samples);
        line.positionCount = visible;

        for (int i = 0; i < visible; i++)
            line.SetPosition(i, points[i]);

        line.startWidth = 0;
        line.endWidth = maxWidth;

        if (t >= 1f) playing = false;
    }

    Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }
}
