using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 720f; // Degrees per second

    [Header("Jump")]
    public float jumpForce = 8f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private bool jumpPressed;
    private bool isGrounded;

    public float Yaw { get; private set; } // Player rotation around Y

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        // --- Movement input ---
        float h = Input.GetAxisRaw("Horizontal"); // Q/D or A/D
        float v = Input.GetAxisRaw("Vertical");   // Z/S or W/S
        moveInput = new Vector3(h, 0f, v).normalized;

        // --- Jump input ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            jumpPressed = true;

        // --- Mouse horizontal rotation ---
        float mouseX = Input.GetAxis("Mouse X");
        Yaw += mouseX * 3f; // Adjust sensitivity here
    }

    void FixedUpdate()
    {
        // --- Rotate player to match mouse ---
        Quaternion targetRot = Quaternion.Euler(0f, Yaw, 0f);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));

        // --- Move relative to facing ---
        Vector3 moveDir = transform.forward * moveInput.z + transform.right * moveInput.x;
        moveDir.Normalize();

        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = rb.linearVelocity.y; // preserve Y
        rb.linearVelocity = velocity;

        // --- Jump ---
        if (jumpPressed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset Y
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;
        }

        // --- Punchy gravity ---
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
    }

    // --- Ground check ---
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
