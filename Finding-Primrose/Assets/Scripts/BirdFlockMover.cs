using UnityEngine;

public class BirdFlockMover : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;

    private int currentWaypointIndex = 0;
    private bool isFlying = false;

    void Update()
    {
        if (!isFlying || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance <= 0.3f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                StopFlying();
                return;
            }

            return;
        }

        direction.Normalize();

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 5f
        );

        transform.position += direction * speed * Time.deltaTime;
    }

    public void StartFlying()
    {
        isFlying = true;
        currentWaypointIndex = 0;

        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            anim.SetBool("flying", true);
        }
    }

    public void StopFlying()
    {
        isFlying = false;

        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            anim.SetBool("flying", false);
        }
    }
}
