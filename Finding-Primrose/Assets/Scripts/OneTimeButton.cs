using UnityEngine;
using UnityEngine.UI;

public class OneTimeButton : MonoBehaviour
{
    private Button button;
    private bool hasBeenClicked = false;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    void HandleClick()
    {
        if (hasBeenClicked) return;

        hasBeenClicked = true;

        gameObject.SetActive(false);
    }
}
