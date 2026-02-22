using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScentSource : MonoBehaviour
{
    [Header("Identity")]
    public string scentName = "Food Trucks";
    public Color scentColor = new Color(1f, 0.55f, 0f);

    [Header("Particles")]
    public ParticleSystem scentParticles;
    public float idleEmissionRate = 4f;     // always drifting gently
    public float detectionRadius  = 10f;    // how far away Primrose can smell it

    [HideInInspector] public float proximity = 0f;

    void Start()
    {
        if (scentParticles == null) return;

        // Set color on the particle system to match this source
        var main = scentParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(scentColor.r, scentColor.g, scentColor.b, 0.6f),
            new Color(scentColor.r, scentColor.g, scentColor.b, 0.2f)
        );

        // Start idle emission
        var emission = scentParticles.emission;
        emission.rateOverTime = idleEmissionRate;
        scentParticles.Play();
    }

    // Called every frame by SniffSystem
    public void UpdateProximity(Vector3 primrosePos)
    {
        float dist = Vector3.Distance(transform.position, primrosePos);
        proximity = 1f - Mathf.Clamp01(dist / detectionRadius);
    }


    void SetParticleColor(Color color, float maxAlpha, float minAlpha)
    {
        var main = scentParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(color.r, color.g, color.b, maxAlpha),
            new Color(color.r, color.g, color.b, minAlpha)
        );
    }


    // Called when Primrose sniffs – fires a burst proportional to proximity
    public void Burst(int maxParticles, Vector3 primrosePos, Color blendedColor)
    {
        if (scentParticles == null || proximity <= 0f) return;

        int count = Mathf.RoundToInt(maxParticles * proximity);
        if (count <= 0) return;

        // Tint to blended color for the burst
        SetParticleColor(blendedColor, 0.9f, 0.5f);

        // Fling toward Primrose
        Vector3 dir = (primrosePos - transform.position).normalized;
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.velocity = dir * 2.5f;

        scentParticles.Emit(emitParams, count);

        // Restore idle color
       // SetParticleColor(scentColor, 0.6f, 0.2f);
    }
}
