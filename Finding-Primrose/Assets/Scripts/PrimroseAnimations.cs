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

    public void SetCautiousCrouch(bool state)
    {
        animator.SetBool("CautiousCrouch", state);
    }

    public void SetCautiousWalk(bool state)
    {
        animator.SetBool("CautiousWalk", state);
    }

}
