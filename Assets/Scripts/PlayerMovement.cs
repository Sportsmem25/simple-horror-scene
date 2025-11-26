using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public LayerMask groundMask;

    private CharacterController controller;
    private float gravity = -9.81f;
    private float rotationX = 0f;
    private float groundCheckDistance = 0.4f;
    private bool isCanLook = true;
    private bool isGrounded;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        LockCursor();
    }

    private void Update()
    {
        GroundCheck();
        Move();
        Look();
        ApplyGravity();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        isCanLook = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void GroundCheck()
    {
        // Is the player standing on the ground
        isGrounded = Physics.CheckSphere(transform.position - Vector3.up * (controller.height / 2 - 0.1f),
                                         groundCheckDistance, groundMask);

        // If there is residual downward speed on the ground, we reset it.
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void Look()
    {
        if (!isCanLook) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}