using UnityEngine;

public class SandwichLandDetector : MonoBehaviour
{
    public GameObject primrose;
    public float eatDistance = .4f;
    private bool landed = false;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Sandwich hit something: " + collision.gameObject.name);
        landed = true;
    }

    void Update()
    {
        if (!landed || primrose == null) return;
        
        float distance = Vector3.Distance(primrose.transform.position, transform.position);
        if (distance < eatDistance)
        {
         

            Animator anim = primrose.GetComponent<Animator>();
            Debug.Log("Animator found: " + anim);
            Debug.Log("Setting Eat trigger on: " + primrose.name);
            anim.SetTrigger("Eat");
            gameObject.SetActive(false);
            landed = false;

        }
    }
}