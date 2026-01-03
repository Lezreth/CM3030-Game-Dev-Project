using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTintPulse : MonoBehaviour
{
    public Image tintImage;

    [Header("Timing")]
    public float fadeInTime = 0.15f;
    public float holdTime = 0.4f;
    public float fadeOutTime = 0.25f;

    [Header("Intensity")]
    [Range(0f, 1f)]
    public float maxAlpha = 0.35f;

    Coroutine running;

    public void Pulse(Color color)
    {
        if (running != null)
            StopCoroutine(running);

        running = StartCoroutine(PulseRoutine(color));
    }

    IEnumerator PulseRoutine(Color color)
    {
        color.a = 0f;
        tintImage.color = color;

        // Fade in
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            float u = t / fadeInTime;
            color.a = Mathf.Lerp(0f, maxAlpha, u);
            tintImage.color = color;
            yield return null;
        }

        color.a = maxAlpha;
        tintImage.color = color;

        yield return new WaitForSeconds(holdTime);

        // Fade out
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            float u = t / fadeOutTime;
            color.a = Mathf.Lerp(maxAlpha, 0f, u);
            tintImage.color = color;
            yield return null;
        }

        color.a = 0f;
        tintImage.color = color;
        running = null;
    }
}
