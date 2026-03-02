
using UnityEngine;

public class SandwichHighlight : MonoBehaviour
{
    private Renderer sandwichRenderer;
    private Color originalColor;

    void Start()
    {
        sandwichRenderer = GetComponent<Renderer>();
        originalColor = sandwichRenderer.material.color;
    }

    void OnMouseEnter()
    {
        sandwichRenderer.material.color = Color.yellow;
    }

    void OnMouseExit()
    {
        sandwichRenderer.material.color = originalColor;
    }
}