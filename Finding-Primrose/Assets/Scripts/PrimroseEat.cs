using UnityEngine;

public class PrimroseEat : MonoBehaviour
{
    public GameObject sandwich;
    public float eatDistance = 1.5f;
    private bool hasEaten = false;
    private bool sandwichThrown = false;

    public void ActivateEating()
    {
        sandwichThrown = true;
        hasEaten = false;
    }

    void Update()
    {
        if (!sandwichThrown || hasEaten || sandwich == null || !sandwich.activeInHierarchy) return;
        
        float distance = Vector3.Distance(transform.position, sandwich.transform.position);
        
        if (distance < eatDistance)
        {
            hasEaten = true;
            sandwichThrown = false;
            sandwich.SetActive(false);
        }
    }
}