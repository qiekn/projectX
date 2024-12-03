using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour {
    [SerializeField] float sensX;
    [SerializeField] float sensY;
    [SerializeField] Transform player;
    [SerializeField] Transform glasses;

    float xRotation;
    float yRotation;

    InputAction moveAction; // Input System workflow - Actions

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        moveAction = InputSystem.actions.FindAction("Look");
    }

    void Update() {
        // Get mouse input
        Vector2 mouseValue = moveAction.ReadValue<Vector2>();
        // Accumulate variables
        yRotation += mouseValue.x * Time.deltaTime * sensX; // Look left or right
        xRotation -= mouseValue.y * Time.deltaTime * sensY; // Look up or down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        // Rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        player.rotation = Quaternion.Euler(0, yRotation, 0);
        glasses.rotation = Quaternion.Euler(Mathf.Clamp(xRotation, -30f, 30f), yRotation, 0); // Vividly rotate glasses
    }

}