using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f; // affects Slerp speed; tweak for smoother/faster rotation
    private Quaternion lastMoveRotation;


    [Header("Jump")]
    public float jumpForce = 8f;
    public float fallMultiplier = 2.5f;
    private bool jumpPressed;
    private bool isGrounded;
    public float riseMultiplier = 1.3f; // gravity while going up


    [Header("References")]
    public Transform playerCam; // assign camera in inspector or use Camera.main
    private Animator animator;

    private Rigidbody rb;
    private Vector3 moveInput;

    [HideInInspector] public float Yaw; // rotation driven by mouse

    // --- Smooth movement direction ---
    private Vector3 currentMoveDir = Vector3.forward;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>(true);

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Auto-assign camera if not set
        if (playerCam == null && Camera.main != null)
            playerCam = Camera.main.transform;

        Yaw = transform.eulerAngles.y;
        lastMoveRotation = transform.rotation;
    }

    void Update()
    {
        if (!GameSceneManager.Instance.inputEnabled)
        return;

        // --- Movement input ---
        float h = Input.GetAxisRaw("Horizontal"); // Q/D or A/D
        float v = Input.GetAxisRaw("Vertical");   // Z/S or W/S
        moveInput = new Vector3(h, 0f, v).normalized;

        bool isMoving = moveInput.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        // --- Jump input ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !jumpPressed)
        {
            jumpPressed = true;
            animator.SetTrigger("jump");
        }

        animator.SetBool("isGrounded", isGrounded);

        // --- Mouse rotation ---
        float mouseX = Input.GetAxis("Mouse X");
        Yaw += mouseX * 3f; // adjust sensitivity
    }

    void FixedUpdate()
    {
        if (!GameSceneManager.Instance.inputEnabled)
        return;
        
        // --- Determine target movement direction ---
        Vector3 targetMoveDir = Vector3.zero;

        if (moveInput.magnitude > 0.1f)
        {
            Vector3 camForward = new Vector3(playerCam.forward.x, 0f, playerCam.forward.z).normalized;
            Vector3 camRight = new Vector3(playerCam.right.x, 0f, playerCam.right.z).normalized;
            targetMoveDir = (camForward * moveInput.z + camRight * moveInput.x).normalized;
        }

        // --- Smooth current movement direction ---
        if (targetMoveDir != Vector3.zero)
            currentMoveDir = Vector3.Slerp(currentMoveDir, targetMoveDir, 0.2f); // tweak 0.2f for smoother/faster transitions

        // --- Determine target rotation ---
        Quaternion targetRot;

        if (moveInput.magnitude > 0.1f)
        {
            targetRot = Quaternion.LookRotation(currentMoveDir);
            lastMoveRotation = targetRot; // store last valid movement rotation
        }
        else
        {
            targetRot = lastMoveRotation; // keep last direction, no mouse reset
        }


        // --- Smoothly rotate the player ---
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));

        // --- Camera-relative movement ---
        Vector3 camForwardMove = new Vector3(playerCam.forward.x, 0f, playerCam.forward.z).normalized;
        Vector3 camRightMove = new Vector3(playerCam.right.x, 0f, playerCam.right.z).normalized;
        Vector3 moveVelocity = (camForwardMove * moveInput.z + camRightMove * moveInput.x).normalized;

        // --- Move player smoothly ---
        Vector3 targetPos = rb.position + moveVelocity * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);

        // --- Jump ---
        if (jumpPressed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;
        }

        // --- Jump gravity shaping (constant height) ---
        if (rb.linearVelocity.y > 0)
        {
            // Going up → slightly heavier gravity (less floaty)
            rb.linearVelocity += Vector3.up * Physics.gravity.y
                * (riseMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y < 0)
        {
            // Falling → heavier gravity
            rb.linearVelocity += Vector3.up * Physics.gravity.y
                * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }

    }

    // --- Ground detection ---
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
