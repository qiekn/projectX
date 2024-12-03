using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text_speed; // Text

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float airControll = 0.5f;
    [SerializeField] float groundDrag;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask groundMask;

    Rigidbody rb;
    InputAction moveAction;
    InputAction jumpAction;

    float moveX, moveY;

    bool canJump = true;
    bool grounded;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update() {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        InputHanlder();
        Move();
        SpeedControl();

        // handle drag
        if (grounded) {
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = 0;
        }
    }

    void InputHanlder() {
        // Look
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        moveX = moveValue.x;
        moveY = moveValue.y;

        // Jump
        if (jumpAction.IsInProgress() && canJump && grounded) {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void Move() {
        var moveDirection = transform.forward * moveY + transform.right * moveX;

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airControll, ForceMode.Force);
    }

    void Jump() {
        // Reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Debug.Log("Jumpped");
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump() {
        canJump = true;
    }

    void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed) {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        text_speed.SetText("Speed: " + flatVel.magnitude.ToString("F2"));
    }

}
