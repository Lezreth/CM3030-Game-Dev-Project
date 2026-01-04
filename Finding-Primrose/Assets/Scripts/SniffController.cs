using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SniffController : MonoBehaviour
{
    [Header("Refs")]
    public Animator dogAnimator;
    public TMP_Text sniffText; 
    public CanvasGroup ribbonFx;
    
    [Header("Camera Zoom")]
    public Camera mainCamera;
    public float zoomFOV = 40f;
    public float normalFOV = 60f;
    public float zoomSpeed = 3f;
    
    [Header("Scent Particles")]
    public ParticleSystem redScent;
    public ParticleSystem greenScent;
    public ParticleSystem blueScent;

    [Header("Sniff Settings")]
    public string sniffTriggerName = "Sniff";
    public float sniffDuration = 2.6f;

    bool isSniffing;
    float sniffTimer;

    void Start()
    {
        SetUIReady(true);
        SetRibbons(false, instant: true);
        
        if (mainCamera != null && normalFOV == 60f)
            normalFOV = mainCamera.fieldOfView;
    }

    void Update()
    {
        // Start sniff on spacebar
        if (!isSniffing && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartSniff();
        }
        
        // Count down sniff timer
        if (isSniffing)
        {
            sniffTimer += Time.deltaTime;
            if (sniffTimer >= sniffDuration)
            {
                EndSniff();
            }
        }
        
        // Smooth camera zoom
        if (mainCamera != null)
        {
            float targetFOV = isSniffing ? zoomFOV : normalFOV;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }

    void StartSniff()
    {
        isSniffing = true;
        sniffTimer = 0f;
        SetUIReady(false);
        
        if (dogAnimator != null)
            dogAnimator.SetTrigger(sniffTriggerName);
        
        SetRibbons(true, instant: false);
        
        if (redScent != null) redScent.Play();
        if (greenScent != null) greenScent.Play();
        if (blueScent != null) blueScent.Play();
    }

    void EndSniff()
    {
        isSniffing = false;
        SetUIReady(true);
        SetRibbons(false, instant: false);
        
        if (redScent != null) redScent.Stop();
        if (greenScent != null) greenScent.Stop();
        if (blueScent != null) blueScent.Stop();
    }

    void SetUIReady(bool ready)
    {
        if (sniffText != null)
            sniffText.enabled = ready;
    }

    void SetRibbons(bool on, bool instant)
    {
        if (ribbonFx == null) return;
        
        ribbonFx.gameObject.SetActive(true);
        ribbonFx.alpha = on ? 1f : 0f;
        
        if (!on) ribbonFx.gameObject.SetActive(false);
    }
}