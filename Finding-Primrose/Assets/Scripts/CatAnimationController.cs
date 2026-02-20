using UnityEngine;

public class CatAnimationController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartRun()
    {
        animator.SetBool("catRun", true);
    }

    public void StopRun()
    {
        animator.SetBool("catRun", false);
    }
}