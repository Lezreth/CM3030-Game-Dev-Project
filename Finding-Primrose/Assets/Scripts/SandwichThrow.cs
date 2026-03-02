using UnityEngine;

public class SandwichThrow : MonoBehaviour
{
    public GameObject sandwichOriginal;
    public GameObject sandwichThrown;
    public GameObject sandwichLanded;
    public Transform handBone;
    public GameObject primrose;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        if (sandwichThrown != null)
        {
            sandwichThrown.SetActive(false);
            Rigidbody rb = sandwichThrown.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }
        
        if (sandwichLanded != null)
            sandwichLanded.SetActive(false);
    }

    public void ThrowSandwich()
    {
        animator.SetTrigger("Throw");
    }

    public void GrabSandwich()
    {
        sandwichOriginal.SetActive(false);
        sandwichThrown.SetActive(true);
        sandwichThrown.transform.SetParent(handBone);
        sandwichThrown.transform.localPosition = Vector3.zero;
        sandwichThrown.transform.localRotation = Quaternion.identity;

        Rigidbody rb = sandwichThrown.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    public void ReleaseSandwich()
    {
        sandwichThrown.transform.SetParent(null);

        Rigidbody rb = sandwichThrown.GetComponent<Rigidbody>();
        if (rb == null) rb = sandwichThrown.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = new Vector3(0, 1f, -.5f);
    }

   public void SandwichLands()
{
    
    
    if (primrose != null)
        primrose.GetComponent<Animator>().SetTrigger("CautiousWalk");
}

    public void ResetSandwich()
    {
        sandwichOriginal.SetActive(true);
        sandwichThrown.SetActive(false);
        sandwichLanded.SetActive(false);
        sandwichThrown.transform.SetParent(null);

        Rigidbody rb = sandwichThrown.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }
}