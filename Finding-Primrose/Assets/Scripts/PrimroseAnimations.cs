using UnityEngine;

public class PrimroseAnimationController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerEat()
    {
        animator.SetTrigger("Eat");
    }

}
