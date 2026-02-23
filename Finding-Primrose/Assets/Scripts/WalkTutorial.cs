using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class WalkTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI label;



    private bool clicked = false;

    void Start()
    {

         if (PlayerPrefs.GetInt("WalkTutorialSeen", 0) == 1)
        {
            Destroy(gameObject);
            return;
        }

        label.text = "";
        label.color = new Color(1f, 0.85f, 0.1f);
        StartCoroutine(IntroSequence());
    }

    void Update()
    {
        if (clicked) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            clicked = true;
            StopAllCoroutines();
            StartCoroutine(PostClickSequence());
        }
    }

    // --- Sequences ---

    IEnumerator IntroSequence()
    {
        yield return StartCoroutine(FadeTextIn("It's very early and you're alone and hungry."));
        yield return new WaitForSeconds(2.2f);
        yield return StartCoroutine(FadeTextOut());
        yield return new WaitForSeconds(0.4f);
        yield return StartCoroutine(FadeTextIn("Click the ground to move."));
    }

    IEnumerator PostClickSequence()
    {

        PlayerPrefs.SetInt("WalkTutorialSeen", 1);
        PlayerPrefs.Save();
        // fade out whatever text is showing
        yield return StartCoroutine(FadeTextOut(0.3f));
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(FadeTextIn("Great, explore the area."));
        yield return new WaitForSeconds(2.2f);
        yield return StartCoroutine(FadeTextOut());
        yield return new WaitForSeconds(0.4f);

        yield return StartCoroutine(FadeTextIn("Good luck and be careful!"));
        yield return new WaitForSeconds(2.5f);

        yield return StartCoroutine(FadeTextOut());
        Destroy(gameObject);
    }

    // --- Text Helpers ---

    IEnumerator FadeTextIn(string message, float duration = 0.6f)
    {
        label.text = message;
        label.color = new Color(label.color.r, label.color.g, label.color.b, 0f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            label.color = new Color(label.color.r, label.color.g, label.color.b,
                          Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
    }

    IEnumerator FadeTextOut(float duration = 0.5f)
    {
        float startAlpha = label.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            label.color = new Color(label.color.r, label.color.g, label.color.b,
                          Mathf.Lerp(startAlpha, 0f, elapsed / duration));
            yield return null;
        }
    }
}