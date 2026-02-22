// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.UI;
// using System.Collections;

// public class SniffSystem : MonoBehaviour
// {
//     [Header("Animator")]
//     public Animator dogAnimator;
//     public string sniffTrigger = "Sniff";

//     [Header("Camera Zoom")]
//     public Camera mainCamera;
//     public float zoomFOV   = 40f;
//     public float normalFOV = 60f;
//     public float zoomSpeed = 3f;

//     [Header("World Split")]
//     public float worldCentreX   = 0f;
//     public float worldHalfWidth = 20f;

//     [Header("Screen Edge Emitters")]
//     [Tooltip("Orange prefab — material already set to FF8C00, soft circle, additive")]
//     public ParticleSystem orangeEmitterPrefab;   // food trucks – right side

//     [Tooltip("Green prefab — material already set to 00FF14, soft circle, additive")]
//     public ParticleSystem greenEmitterPrefab;    // lake – left side

//     public int   burstCount    = 25;
//     public float particleSpeed = 1.5f;
//     public float particleLife  = 3f;
//     public float holdDuration  = 0.6f;
//     public float emitterDepth  = 5f;

//     [Header("UI")]
//     public Image sniffButtonImage;

//     [Header("Sniff Settings")]
//     public float sniffDuration = 2.6f;

//     private bool isSniffing;
//     private float sniffTimer;
//     public bool IsSniffing => isSniffing;

//     void Start()
//     {
//         if (mainCamera != null) normalFOV = mainCamera.fieldOfView;
//     }

//     void Update()
//     {
//         float blend = GetWorldBlend();

//         // Tint single button between green and orange based on world position
//         if (sniffButtonImage != null)
//         {
//             Color orange = new Color(1f, 0.549f, 0f);
//             Color green  = new Color(0f, 1f, 0.078f);
//             sniffButtonImage.color = Color.Lerp(green, orange, blend);
//         }

//         if (!isSniffing && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
//             TriggerSniff();

//         if (isSniffing)
//         {
//             sniffTimer += Time.deltaTime;
//             if (sniffTimer >= sniffDuration) EndSniff();
//         }

//         if (mainCamera != null)
//         {
//             float targetFOV = isSniffing ? zoomFOV : normalFOV;
//             mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
//         }
//     }

//     float GetWorldBlend()
//     {
//         float offset = transform.position.x - worldCentreX;
//         return Mathf.Clamp01((offset + worldHalfWidth) / (worldHalfWidth * 2f));
//     }

//     public void TriggerSniff()
//     {
//         if (!isSniffing) StartSniff();
//     }

//     void StartSniff()
//     {
//         isSniffing = true;
//         sniffTimer = 0f;

//         if (dogAnimator != null)
//             dogAnimator.SetTrigger(sniffTrigger);

//         StartCoroutine(FireEdgeEmitters(GetWorldBlend()));
//     }

//     void EndSniff()
//     {
//         isSniffing = false;
//     }

//     IEnumerator FireEdgeEmitters(float blend)
//     {
//         int orangeCount = Mathf.RoundToInt(burstCount * blend);
//         int greenCount  = Mathf.RoundToInt(burstCount * (1f - blend));

//         if (orangeCount > 0 && orangeEmitterPrefab != null)
//         {
//             ParticleSystem ps = SpawnEmitter(RightEdgePos(), Vector3.left);
//             // NO color code — prefab material handles it
//             ps.Emit(orangeCount);
//             Destroy(ps.gameObject, particleLife + holdDuration);
//         }

//         if (greenCount > 0 && greenEmitterPrefab != null)
//         {
//             ParticleSystem ps = SpawnEmitter(LeftEdgePos(), Vector3.right);
//             ps.Emit(greenCount);
//             Destroy(ps.gameObject, particleLife + holdDuration);
//         }

//         yield return null;
//     }

//     ParticleSystem SpawnEmitter(Vector3 worldPos, Vector3 direction)
//     {
//         // Use the correct prefab per side
//         ParticleSystem prefab = direction == Vector3.left ? orangeEmitterPrefab : greenEmitterPrefab;
//         ParticleSystem ps = Instantiate(prefab, worldPos, Quaternion.LookRotation(direction));

//         // Only set speed and lifetime — NOT color
//         var main = ps.main;
//         main.startSpeed    = particleSpeed;
//         main.startLifetime = particleLife;

//         return ps;
//     }

//     Vector3 RightEdgePos()
//     {
//         return mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, emitterDepth));
//     }

//     Vector3 LeftEdgePos()
//     {
//         return mainCamera.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2f, emitterDepth));
//     }
// }


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
        // Tint button by world position
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