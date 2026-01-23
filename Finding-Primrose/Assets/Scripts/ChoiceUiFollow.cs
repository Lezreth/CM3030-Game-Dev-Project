using UnityEngine;
// Positions the Choice UI above primroses head 
public class ChoiceUIFollow : MonoBehaviour
{
    public Transform target; // primrose
    public Vector3 offset = new Vector3(0, 2, 0); // above head
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (target == null || rect == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);

        // Only if in front of camera
        if (screenPos.z > 0)
        {
            rect.position = screenPos;
        }
    }
}


