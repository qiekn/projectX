using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    #region Field

    [SerializeField] TextMeshProUGUI text_speed; // Text
    [SerializeField] TextMeshProUGUI text_slope; // Text

    [Header("Movement")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float airControll = 0f;
    [SerializeField] float groundDrag = 8f;
    float moveX;
    float moveY;
    Vector3 moveDirection;

    [Header("Jump")]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float jumpCooldown = 0.25f;
    bool canJump = true;

    [Header("Crouch")]
    [SerializeField] float crouchSpeedMulti = 0.6f;
    [SerializeField] Vector3 crouchScale = new Vector3(1f, 0.5f, 1f);
    Vector3 playerScale;

    [Header("Sprint")]
    [SerializeField] float sprintSpeedMulti = 2f;
    [SerializeField] float sprintFOVMulti = 1.1f;

    [Header("Slide")]
    [SerializeField] float thresholdSpeedForSlide = 5f;
    [SerializeField] float slideForce = 5f;
    [SerializeField] float slideSpeedMulti = 2f;
    [SerializeField] float slideFOVMulti = 1.1f;

    [Header("Ground Check")]
    [SerializeField] float playerHeight = 2f;
    [SerializeField] LayerMask groundMask;
    bool grounded;

    [Header("Slope Check")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopHit;
    bool sloped;
    float slopeAngle;

    public Camera playerCam;
    public MovementState state;
    public enum MovementState {
        walking,
        sprinting,
        crouching,
        sliding
    }

    float maxSpeed;
    float baseSpeed;

    Rigidbody rb;
    InputAction moveAction, jumpAction, sprintAction, crouchAction, slideAction;
    bool moving, jumping, sprinting, crouching, sliding;

    #endregion

    #region Unity

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        MapActions();
        baseSpeed = walkSpeed;
        playerScale = transform.localScale;
    }

    void Update() {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        // slope check
        sloped = OnSlop();

        DragHandler();  // Apply Drag
        InputHandler(); // Reponse to input
        SpeedHandler(); // Swtich movement state
        SpeedLimiter(); // Limit move speed
    }

    #endregion

    #region Methods

    void MapActions() {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        slideAction = InputSystem.actions.FindAction("Slide");
    }

    void DragHandler() {
        if (sloped) {
            rb.linearDamping = groundDrag * 1.2f;
        } else if (grounded) {
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = 0;
        }
    }

    void InputHandler() {
        // Move
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Move(moveValue);

        // Jump
        if (jumpAction.IsInProgress()) {
            Jump();
        }

        // Crouch
        if (crouchAction.IsPressed()) {
            StartCrouch();
        } else if (crouchAction.WasReleasedThisFrame()) {
            StopCrouch();
        }

        // Sprint
        if (sprintAction.IsPressed()) {
            StartSprint();
        } else if (sprintAction.WasReleasedThisFrame()) {
            StopSprint();
        }

        // Slide
        if (slideAction.IsPressed()) {
            StartSlide();
        } else if (slideAction.WasReleasedThisFrame()) {
            StopSlide();
        }
    }

    void Move(Vector2 moveValue) {
        moveX = moveValue.x;
        moveY = moveValue.y;
        moveDirection = transform.forward * moveY + transform.right * moveX;

        // Apply Force
        if (sloped) {
            rb.AddForce(GetSlopMoveDirection() * maxSpeed * 20f, ForceMode.Force);
            if (rb.linearVelocity.y > 0) {
                rb.AddForce(-1 * slopHit.normal.normalized * 80f, ForceMode.Force);
            }
        } else if (grounded) {
            rb.AddForce(moveDirection.normalized * maxSpeed * 10f, ForceMode.Force);
        } else if (!grounded) {
            rb.AddForce(moveDirection.normalized * maxSpeed * 10f * airControll, ForceMode.Force);
        }
        rb.useGravity = !sloped;
    }

    void Jump() {
        if (canJump && grounded) {
            canJump = false;
            // Reset y velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump() {
        canJump = true;
    }

    void StartCrouch() {
        if (!crouching) {
            crouching = true;
            state = MovementState.crouching;
            transform.localScale = crouchScale;
            rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        }
    }

    void StopCrouch() {
        if (crouching) {
            crouching = false;
            state = MovementState.walking;
            transform.localScale = playerScale;
        }
    }

    void StartSprint() {
        if (!sprinting && moveY > 0) {
            sprinting = true;
            state = MovementState.sprinting;
            playerCam.fieldOfView *= sprintFOVMulti;
        }
    }

    void StopSprint() {
        if (sprinting) {
            sprinting = false;
            state = MovementState.walking;
            playerCam.fieldOfView /= sprintFOVMulti;
        }
    }

    void StartSlide() {
        if (!sliding && grounded && rb.linearVelocity.magnitude > thresholdSpeedForSlide) {
            sliding = true;
            state = MovementState.sliding;
            transform.localScale = crouchScale;
            rb.AddForce(transform.forward * slideForce);
        }
    }

    void StopSlide() {
        if (sliding) {
            sliding = false;
            state = MovementState.walking;
            transform.localScale = playerScale;
        }
    }

    void SpeedHandler() {
        // Specially handle slope case --> speed mapped to flat speed
        walkSpeed = sloped ? baseSpeed * Mathf.Cos(Mathf.Deg2Rad * slopeAngle) : baseSpeed;

        // Handle Different Speed States
        switch (state) {
            case MovementState.crouching:
                maxSpeed = walkSpeed * crouchSpeedMulti;
                break;
            case MovementState.sprinting:
                maxSpeed = walkSpeed * sprintSpeedMulti;
                break;
            case MovementState.sliding:
                maxSpeed = walkSpeed * slideSpeedMulti;
                break;
            case MovementState.walking:
            default:
                maxSpeed = walkSpeed;
                break;
        }
    }

    void SpeedLimiter() {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        // limit velocity if needed
        if (flatVel.magnitude > maxSpeed) {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        text_speed.SetText("Speed: " + flatVel.magnitude.ToString("F2"));
    }

    bool OnSlop() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopHit, playerHeight * .5f + .3f)) {
            slopeAngle = Vector3.Angle(Vector3.up, slopHit.normal);
            text_slope.SetText("Slope: " + slopeAngle.ToString("F2"));
            return slopeAngle <= maxSlopeAngle && slopeAngle != 0;
        }
        return false;
    }

    Vector3 GetSlopMoveDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopHit.normal).normalized;
    }

    #endregion
}