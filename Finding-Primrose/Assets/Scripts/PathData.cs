using UnityEngine;

public class PathData : MonoBehaviour
{
    //array for waypoints 
    public Transform[] waypoints;
    //for visualizing - does not show at run time 
    public Color pathColor = Color.yellow;
    public bool showGizmos = true;
    
    void OnDrawGizmos()
    {
        if (!showGizmos || waypoints == null || waypoints.Length < 2)
            return;
            
        Gizmos.color = pathColor;
        
        // Draw lines between waypoints
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawWireSphere(waypoints[i].position, 0.2f);
            }
        }
        
        // Draw last waypoint
        if (waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawWireSphere(waypoints[waypoints.Length - 1].position, 0.2f);
        }
    }
    
    public Vector3 GetClosestPointOnPath(Vector3 clickPosition)
    {
        if (waypoints == null || waypoints.Length == 0)
            return transform.position;
            
        Vector3 closest = waypoints[0].position;
        float minDist = float.MaxValue;
        
        // Check each segment of the path
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector3 segmentPoint = GetClosestPointOnSegment(
                waypoints[i].position, 
                waypoints[i + 1].position, 
                clickPosition
            );
            
            float dist = Vector3.Distance(clickPosition, segmentPoint);
            if (dist < minDist)
            {
                minDist = dist;
                closest = segmentPoint;
            }
        }
        

        return closest;
    }
    
    Vector3 GetClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(point - a, ab) / Vector3.Dot(ab, ab);
        t = Mathf.Clamp01(t);
        Debug.Log("t = " + t);
        Debug.Log("Closest Point = " + (a + ab * t));
        return a + ab * t;
    }
    

    public bool IsNearPathEnd(Vector3 position, float threshold = 1f)
    {
        if (waypoints == null || waypoints.Length == 0)
            return false;
            
        float distToStart = Vector3.Distance(position, waypoints[0].position);
        float distToEnd = Vector3.Distance(position, waypoints[waypoints.Length - 1].position);
        
        return distToStart < threshold || distToEnd < threshold;
    }
}