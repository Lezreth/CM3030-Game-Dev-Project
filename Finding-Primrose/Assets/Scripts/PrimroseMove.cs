using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float groundCheckDistance = 0.2f;

    public bool canMove = true;

    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Rigidbody rb;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;

        //inputActions.Player.Jump.performed += _ => Jump();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, 0f);
        //I couldn't get her to move well, so i restricted the W/S
        //rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.y * moveSpeed
    }

    //void Jump()
    //{
    //    if (!canMove)
    //        return;

    //    if (IsGrounded())
    //    {
    //        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    //        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //    }
    //}

    //bool IsGrounded()
    //{
    //    return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    //}
}
