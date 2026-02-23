


using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class SniffSystem : MonoBehaviour
{
    [Header("Animator")]
    public Animator dogAnimator;
    public string sniffTrigger = "Sniff";

    [Header("Camera Zoom")]
    public Camera mainCamera;
    public float zoomFOV   = 40f;
    public float normalFOV = 60f;
    public float zoomSpeed = 3f;

    [Header("World Split")]
    public float worldCentreX   = 0f;
    public float worldHalfWidth = 20f;

    [Header("Scent Prefabs")]
    [Tooltip("Drag your left/lake emitter prefab here")]
    public ParticleSystem leftPrefab;

    [Tooltip("Drag your right/food truck emitter prefab here")]
    public ParticleSystem rightPrefab;

    [Header("Spawn Points")]
    [Tooltip("Empty child of Camera at left edge")]
    public Transform leftSpawnPoint;

    [Tooltip("Empty child of Camera at right edge")]
    public Transform rightSpawnPoint;

    [Header("Burst Settings")]
    public int   burstCount   = 30;
    public float destroyAfter = 3f;

    [Header("UI")]
    public Image sniffButtonImage;

    [Header("Sniff Settings")]
    public float sniffDuration = 2.6f;

    private bool isSniffing;
    private float sniffTimer;
    public bool IsSniffing => isSniffing;

    void Start()
    {
        if (mainCamera != null) normalFOV = mainCamera.fieldOfView;
    }

    void Update()
    {
        
        float blend = GetWorldBlend();
        if (sniffButtonImage != null)
            sniffButtonImage.color = Color.Lerp(
                new Color(0f, 1f, 0.078f),   // 00FF14 green  – lake
                new Color(1f, 0.549f, 0f),    // FF8C00 orange – food trucks
                blend
            );

        if (!isSniffing && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            TriggerSniff();

        if (isSniffing)
        {
            sniffTimer += Time.deltaTime;
            if (sniffTimer >= sniffDuration) EndSniff();
        }

        if (mainCamera != null)
        {
            float targetFOV = isSniffing ? zoomFOV : normalFOV;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }

    float GetWorldBlend()
    {
        float offset = transform.position.x - worldCentreX;
        return Mathf.Clamp01((offset + worldHalfWidth) / (worldHalfWidth * 2f));
    }

    public void TriggerSniff()
    {
        if (!isSniffing) StartSniff();
    }

    void StartSniff()
    {
        isSniffing = true;
        sniffTimer = 0f;
        Debug.Log($"[SniffSystem] StartSniff called. Animator: {dogAnimator}, trigger: {sniffTrigger}");

        if (dogAnimator != null)
            dogAnimator.SetTrigger(sniffTrigger);

        float blend = GetWorldBlend();
        int rightCount = Mathf.RoundToInt(burstCount * blend);
        int leftCount  = Mathf.RoundToInt(burstCount * (1f - blend));

        if (leftCount > 0 && leftPrefab != null && leftSpawnPoint != null)
        {
            ParticleSystem ps = Instantiate(leftPrefab, leftSpawnPoint.position, leftSpawnPoint.rotation);
            ps.Emit(leftCount);
            Destroy(ps.gameObject, destroyAfter);
        }

        if (rightCount > 0 && rightPrefab != null && rightSpawnPoint != null)
        {
            ParticleSystem ps = Instantiate(rightPrefab, rightSpawnPoint.position, rightSpawnPoint.rotation);
            ps.Emit(rightCount);
            Destroy(ps.gameObject, destroyAfter);
        }
    }

    void EndSniff()
    {
        isSniffing = false;
    }
}