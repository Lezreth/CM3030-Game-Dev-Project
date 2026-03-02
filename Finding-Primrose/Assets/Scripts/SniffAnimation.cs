using UnityEngine;

public class SniffAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Sniff()
    {
        animator.SetTrigger("Sniff");
    }
}