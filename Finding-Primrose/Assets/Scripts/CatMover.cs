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
        if (!isRunning || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];

        Vector3 flatTargetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 direction = flatTargetPos - transform.position;

        float distance = direction.magnitude;

        if (distance <= 0.2f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                StopRun();
                return;
            }

            return; 
        }

        direction.Normalize();

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 8f
        );

        controller.Move(direction * speed * Time.deltaTime);

        animator.SetBool("catRun", true);
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