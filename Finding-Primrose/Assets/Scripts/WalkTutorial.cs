using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WalkTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image sniffButtonImage;
    [SerializeField] private RectTransform statMetersParent;

    [Header("Debug")]
    [SerializeField] private bool debugAlwaysShow = false;

    private bool clicked = false;

    void Start()
    {
        if (!debugAlwaysShow && PlayerPrefs.GetInt("WalkTutorialSeen", 0) == 1)
        {
            Destroy(gameObject);
            return;
        }

        label.text = "";
        label.color = new Color(1f, 0.85f, 0.1f, 0f);
        StartCoroutine(IntroSequence());
    }

    void Update()
    {
        if (clicked) return;

       
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            clicked = true;
            StopAllCoroutines();
            StartCoroutine(PostClickSequence());
        }
    }

    

    IEnumerator IntroSequence()
    {
        yield return StartCoroutine(FadeTextIn("It's very early and you're alone and hungry."));
        yield return new WaitForSeconds(3.2f);
        yield return StartCoroutine(FadeTextOut());
        yield return new WaitForSeconds(0.4f);
        yield return StartCoroutine(FadeTextIn("Click the ground to move."));
    }

    IEnumerator PostClickSequence()
    {
        PlayerPrefs.SetInt("WalkTutorialSeen", 1);
        PlayerPrefs.Save();

        yield return StartCoroutine(FadeTextOut(0.3f));
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(FadeTextIn("Great, explore the area."));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeTextOut());
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(FadeTextIn("These are your health metrics."));
        if (statMetersParent != null)
            StartCoroutine(PulseUIGroup(statMetersParent, 3f));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadeTextOut());
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(FadeTextIn("Sniffing gives clues about direction."));
        if (sniffButtonImage != null)
            StartCoroutine(PulseButtonColor(sniffButtonImage, 3f));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadeTextOut());
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(FadeTextIn("Try to find food, and learn to trust those you can."));
        yield return new WaitForSeconds(3.5f);
        yield return StartCoroutine(FadeTextOut());
        yield return new WaitForSeconds(0.4f);

        yield return StartCoroutine(FadeTextIn("Good luck and be careful!"));
        yield return new WaitForSeconds(3.5f);
        yield return StartCoroutine(FadeTextOut());
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }



    IEnumerator PulseUIGroup(RectTransform parent, float duration)
    {
 
        Image[] images = parent.GetComponentsInChildren<Image>(true);
        TextMeshProUGUI[] texts = parent.GetComponentsInChildren<TextMeshProUGUI>(true);

        Color[] originalImageColors = new Color[images.Length];
        Color[] originalTextColors  = new Color[texts.Length];

        for (int i = 0; i < images.Length; i++) originalImageColors[i] = images[i].color;
        for (int i = 0; i < texts.Length;  i++) originalTextColors[i]  = texts[i].color;

        Color highlightColor = new Color(1f, 0.85f, 0.1f); 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = (Mathf.Sin(elapsed * 5f) + 1f) / 2f;

            for (int i = 0; i < images.Length; i++)
                images[i].color = Color.Lerp(originalImageColors[i], highlightColor, t);

            for (int i = 0; i < texts.Length; i++)
                texts[i].color = Color.Lerp(originalTextColors[i], highlightColor, t);

            yield return null;
        }

       
        for (int i = 0; i < images.Length; i++) images[i].color = originalImageColors[i];
        for (int i = 0; i < texts.Length;  i++) texts[i].color  = originalTextColors[i];
    }

    

    IEnumerator PulseButtonColor(Image img, float duration)
    {
        Color originalColor = img.color;
        Color highlightColor = new Color(1f, 0.85f, 0.1f); 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = (Mathf.Sin(elapsed * 5f) + 1f) / 2f; 
            img.color = Color.Lerp(originalColor, highlightColor, t);
            yield return null;
        }

        img.color = originalColor; 
    }

   

    IEnumerator FadeTextIn(string message, float duration = 0.6f)
    {
        label.text = message;
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
