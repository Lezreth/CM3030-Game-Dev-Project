using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefPatrolRoutine : MonoBehaviour
{
    [System.Serializable]
    public class PatrolPoint
    {
        public Transform point;

        [Header("Stop Facing")]
        [Tooltip("If enabled, chef will rotate after arriving before playing the action.")]
        public bool setFacing = false;

        [Tooltip("If Use Waypoint Rotation is ON, this value is ignored.")]
        public float facingY = 0f; // degrees

        [Tooltip("If ON, chef uses point.rotation.y as final facing (recommended).")]
        public bool useWaypointRotation = true;

        [Header("Stop Behavior")]
        public bool waitHere = false;
        public float waitSeconds = 0f;

        [Header("Animation Trigger")]
        [Tooltip("Animator trigger to fire after arriving (and after facing, if enabled).")]
        public string triggerName;          // e.g. "DoCleanTable"
        public float triggerDelay = 0f;     // delay after arrival before triggering
    }

    [Header("Route (in order)")]
    public List<PatrolPoint> route = new List<PatrolPoint>();

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.5f;
    public float turnSpeed = 8f;

    [Header("Animation")]
    public Animator animator;

    [Header("Transforms to Move/Rotate")]
    [Tooltip("Transform that moves through the world (often the GameObject this script is on).")]
    public Transform moveTarget;

    [Tooltip("Transform that visually rotates the character (often same as moveTarget).")]
    public Transform rotateTarget;

    private bool isActive = true;

    private Coroutine patrolCoroutine;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (moveTarget == null) moveTarget = transform;
        if (rotateTarget == null) rotateTarget = transform;

        patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
    {
        int i = 0;

        while (isActive && route != null && route.Count > 0)
        {
            PatrolPoint p = route[i];

            if (p.point == null)
            {
                Debug.LogError($"[Chef] Route point {i} is NULL!");
                yield break;
            }

            Debug.Log($"[Chef] Walking to route point {i}: {p.point.name}");
            yield return WalkToWaypoint(p.point);

            // Stop orientation (before action)
            if (p.setFacing)
            {
                float y = p.useWaypointRotation ? p.point.eulerAngles.y : p.facingY;
                yield return RotateToFacing(y);
            }

            // Trigger animation
            if (!string.IsNullOrEmpty(p.triggerName))
            {
                if (p.triggerDelay > 0f)
                    yield return new WaitForSeconds(p.triggerDelay);

                animator.SetTrigger(p.triggerName);
            }

            // Optional wait
            if (p.waitHere && p.waitSeconds > 0f)
                yield return new WaitForSeconds(p.waitSeconds);

            i = (i + 1) % route.Count;
        }
    }

    IEnumerator WalkToWaypoint(Transform waypoint)
    {
        if (waypoint == null)
        {
            Debug.LogError("[Chef] Waypoint is NULL!");
            yield break;
        }

        // Cache target once (prevents jitter if waypoint is moved/parented weirdly)
        Vector3 target = waypoint.position;
        target.y = moveTarget.position.y;

        animator.SetBool("Walking", true);

        while (Vector3.Distance(moveTarget.position, target) > stoppingDistance)
        {
            Vector3 toTarget = target - moveTarget.position;
            Vector3 direction = toTarget.normalized;

            moveTarget.position += direction * moveSpeed * Time.deltaTime;

            // Face movement direction while walking
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(direction);
                rotateTarget.rotation = Quaternion.Slerp(rotateTarget.rotation, look, turnSpeed * Time.deltaTime);
            }

            yield return null;
        }

        animator.SetBool("Walking", false);
    }

    IEnumerator RotateToFacing(float yDegrees)
    {
        Quaternion targetRot = Quaternion.Euler(0f, yDegrees, 0f);

        while (Quaternion.Angle(rotateTarget.rotation, targetRot) > 1f)
        {
            rotateTarget.rotation = Quaternion.Slerp(
                rotateTarget.rotation,
                targetRot,
                turnSpeed * Time.deltaTime
            );
            yield return null;
        }

        rotateTarget.rotation = targetRot;
    }

    // When player triggers the choice
    public void StopPatrol()
    {
        isActive = false;

        if (patrolCoroutine != null)
            StopCoroutine(patrolCoroutine);

        animator.SetBool("Walking", false);
    }
}
