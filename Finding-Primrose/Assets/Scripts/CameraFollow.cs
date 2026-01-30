using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   
    public Transform target; // The dog

    public Vector3 offset = new Vector3(0, 5, -8);
    public float smoothSpeed = 5f;
    public bool followX = true;
    public bool followY = false; 
    public bool followZ = true;
    
    void LateUpdate()
    {
        if (target == null)
            return;
        
        // find desired position
        Vector3 desiredPosition = target.position + offset;
        
        //  follow on different axes
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        //lock axis
        float newX = followX ? smoothedPosition.x : transform.position.x;
        float newY = followY ? smoothedPosition.y : transform.position.y;
        float newZ = followZ ? smoothedPosition.z : transform.position.z;
        
        transform.position = new Vector3(newX, newY, newZ);
    }
}