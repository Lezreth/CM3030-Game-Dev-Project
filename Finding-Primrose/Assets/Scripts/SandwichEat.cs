using UnityEngine;
using System.Collections;

public class SandwichEatTrigger : MonoBehaviour
{
    public Transform mouthTransform; 
    public Animator primroseAnimator;
    public float eatDistance = 0.5f;
    private bool hasTriggered = false;

    void Update()
    {
        if (hasTriggered || mouthTransform == null) return;

        float distance = Vector3.Distance(transform.position, mouthTransform.position);
        

        if (distance < eatDistance)
        {
            hasTriggered = true;
            primroseAnimator.SetTrigger("Eat");
            StartCoroutine(DisableAfterDelay());
        }
    }

    IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}