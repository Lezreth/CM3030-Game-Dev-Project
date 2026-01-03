using UnityEngine;

public class SniffInput : MonoBehaviour
{
    public Animator animator;
    public KeyCode key = KeyCode.Space;
    public string sniffTrigger = "Sniff";

    void Reset()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator != null && Input.GetKeyDown(key))
        {
            animator.SetTrigger(sniffTrigger);
        }
    }
}
