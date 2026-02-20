using UnityEngine;

public class FishDestroyer : MonoBehaviour
{
    //public void RemoveFish()
    //{
    //    Destroy(gameObject);
    //}
    public void RemoveFish()
    {
        gameObject.SetActive(false);
    }
}
