using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;

    public bool canMove = true;

    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => Jump();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        Debug.Log("MoveInput X: " + moveInput.x);
    }

    void Jump()
    {
        if (!canMove)
            return;

        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f) // ground check
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}

