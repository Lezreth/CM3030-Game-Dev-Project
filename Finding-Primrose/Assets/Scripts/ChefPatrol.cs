using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefPatrolRoutine : MonoBehaviour
{
    [System.Serializable]
    public class PatrolPoint
    {
        public Transform point;
        public string animationTrigger;
        public float waitSeconds = 2f;
        public float facingY = 0f;
    }

    [Header("Route")]
    public List<PatrolPoint> route = new List<PatrolPoint>();

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.5f;

    [Header("References")]
    public Animator animator;
    public Transform moveTransform;

    private bool isActive = true;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (moveTransform == null) moveTransform = transform;

        StartCoroutine(PatrolRoutine());
        StartCoroutine(DebugLoop());
    }

    IEnumerator DebugLoop()
    {
        while (true)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            string animName = "Unknown";
            if (state.IsName("Walking")) animName = "Walking";
            else if (state.IsName("Standing_Idle")) animName = "Standing_Idle";
            else if (state.IsName("Cleaning_Sweeping")) animName = "Cleaning_Sweeping";
            else if (state.IsName("Cleaning_Table")) animName = "Cleaning_Table";
            else animName = $"Hash:{state.shortNameHash}";
            string transition = animator.IsInTransition(0) ? " [TRANSITIONING]" : "";
            Debug.Log($"[Chef] Anim: {animName}{transition} | Waypoint: {_debugStatus}");
            yield return new WaitForSeconds(0.5f);
        }
    }

    private string _debugStatus = "starting";

    IEnumerator PatrolRoutine()
    {
        int i = 0;

        while (isActive && route != null && route.Count > 0)
        {
            PatrolPoint p = route[i];
            if (p.point == null) { i = (i + 1) % route.Count; continue; }

            int from = ((i - 1) + route.Count) % route.Count;
            _debugStatus = $"walking {from} -> {i}";

            animator.SetBool("Walking", true);
            while (Vector3.Distance(moveTransform.position, p.point.position) > stoppingDistance)
            {
                Vector3 dir = (p.point.position - moveTransform.position).normalized;
                dir.y = 0;
                moveTransform.position += dir * moveSpeed * Time.deltaTime;
                yield return null;
            }

            animator.SetBool("Walking", false);
            _debugStatus = $"at waypoint {i} ({p.animationTrigger})";

            // Rotate to facing
            Quaternion targetRot = Quaternion.Euler(0, p.facingY, 0);
            Quaternion startRot = moveTransform.rotation;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 5f;
                moveTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            if (!string.IsNullOrEmpty(p.animationTrigger))
            {
                animator.SetTrigger(p.animationTrigger);
                yield return new WaitForSeconds(p.waitSeconds);
            }
            else
            {
                yield return null;
            }

            i = (i + 1) % route.Count;
        }
    }

    public void StopPatrol()
    {
        isActive = false;
        StopAllCoroutines();
        animator.SetBool("Walking", false);
    }

    public void ResumePatrol()
    {
        isActive = true;
        StartCoroutine(PatrolRoutine());
    }
}