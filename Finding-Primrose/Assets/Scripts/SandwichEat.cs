using UnityEngine;

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
        Debug.Log("Distance to mouth: " + distance);

        if (distance < eatDistance)
        {
            hasTriggered = true;
            primroseAnimator.SetTrigger("Eat");
            gameObject.SetActive(false);
        }
    }
}