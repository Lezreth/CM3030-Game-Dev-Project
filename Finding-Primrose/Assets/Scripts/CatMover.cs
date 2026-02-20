using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CatMover : MonoBehaviour
{
    public float speed = 3f;             
    public Transform[] waypoints;        
    private int currentWaypointIndex = 0; 
    private CharacterController controller;
    private Animator animator;
    private bool isRunning = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isRunning && waypoints.Length > 0)
        {
            // Move the cat to waypoint
            Vector3 direction = waypoints[currentWaypointIndex].position - transform.position;
            direction.y = 0; 

            if (direction.magnitude > 0.1f)
            {
                direction.Normalize();
                controller.Move(direction * speed * Time.deltaTime);

                animator.SetBool("catRun", true);
            }
            else
            {
                // stop and go to the next waypoint
                animator.SetBool("catRun", false);
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    StopRun();
                }
            }
        }
    }

    public void StartRun()
    {
        isRunning = true;
        currentWaypointIndex = 0; 
        animator.SetBool("catRun", true);
    }

    public void StopRun()
    {
        isRunning = false;
        animator.SetBool("catRun", false);
    }
}