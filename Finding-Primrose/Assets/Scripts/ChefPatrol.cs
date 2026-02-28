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
    }

    [Header("Route")]
    public List<PatrolPoint> route = new List<PatrolPoint>();

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.5f;

    [Header("References")]
    public Animator animator;
    public Transform moveTransform;
    public Transform rotateTransform;

    private bool isActive = true;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (moveTransform == null)
            moveTransform = transform;
        if (rotateTransform == null)
            rotateTransform = transform;

        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
    {
        int i = 0;

        while (isActive && route != null && route.Count > 0)
        {
            PatrolPoint p = route[i];
            if (p.point == null) { i++; continue; }

            animator.SetBool("Walking", true);
            while (Vector3.Distance(moveTransform.position, p.point.position) > stoppingDistance)
            {
                Vector3 dir = (p.point.position - moveTransform.position).normalized;
                dir.y = 0;
                moveTransform.position += dir * moveSpeed * Time.deltaTime;
                rotateTransform.rotation = Quaternion.Slerp(rotateTransform.rotation,
                    Quaternion.LookRotation(dir), Time.deltaTime * 8f);
                yield return null;
            }

            animator.SetBool("Walking", false);

            if (!string.IsNullOrEmpty(p.animationTrigger) && p.waitSeconds > 0)
            {
                rotateTransform.rotation = Quaternion.Euler(0, p.point.eulerAngles.y, 0);
                animator.SetTrigger(p.animationTrigger);
                yield return new WaitForSeconds(p.waitSeconds);
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
}