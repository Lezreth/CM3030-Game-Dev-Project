using System.Collections.Generic;
using UnityEngine;

public class PatrolPathVisualizer : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    public Color lineColor = Color.yellow;
    public Color arrowColor = Color.cyan;
    public bool loop = true;

    // Mirror the facingY values from ChefPatrolRoutine for visualization
    public List<float> facingAngles = new List<float>();

    void OnDrawGizmos()
    {
        if (points == null || points.Count < 2) return;

        // Draw path lines
        Gizmos.color = lineColor;
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (points[i] == null || points[i + 1] == null) continue;
            Gizmos.DrawLine(points[i].position, points[i + 1].position);
            Gizmos.DrawSphere(points[i].position, 0.1f);
        }
        if (points[points.Count - 1] != null)
            Gizmos.DrawSphere(points[points.Count - 1].position, 0.1f);
        if (loop && points[0] != null && points[points.Count - 1] != null)
            Gizmos.DrawLine(points[points.Count - 1].position, points[0].position);

        // Draw facing arrows
        Gizmos.color = arrowColor;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == null) continue;
            float angle = (i < facingAngles.Count) ? facingAngles[i] : 0f;
            Vector3 origin = points[i].position + Vector3.up * 0.1f;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Main arrow shaft
            Gizmos.DrawLine(origin, origin + dir * 0.6f);

            // Arrowhead
            Vector3 tip = origin + dir * 0.6f;
            Vector3 left  = Quaternion.Euler(0,  145, 0) * dir * 0.25f;
            Vector3 right = Quaternion.Euler(0, -145, 0) * dir * 0.25f;
            Gizmos.DrawLine(tip, tip + left);
            Gizmos.DrawLine(tip, tip + right);

            // Label
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(origin + Vector3.up * 0.2f, $"{i} ({angle}°)");
            #endif
        }
    }
}