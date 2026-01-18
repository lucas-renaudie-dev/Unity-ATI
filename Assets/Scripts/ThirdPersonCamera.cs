using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 8, -8);
    public float mouseSensitivity = 3f;
    public float minDistance = 1.5f;
    public float maxDistance = 8f;

    [Header("Vertical limits")]
    public float minPitch = 10f;
    public float maxPitch = 25f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float cameraRadius = 0.25f;
    public float wallOffset = 0.2f;

    [Header("Smoothing")]
    public float smoothTime = 0.08f;

    private float yaw = 0f;
    private float pitch = 15f;
    private float currentDistance;
    private float distanceVelocity;
    private Vector3 currentVelocity;

    void Start()
    {
        yaw = player.eulerAngles.y;
        currentDistance = offset.magnitude;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // --- Mouse rotation ---
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 forward = rotation * Vector3.forward;

        // --- Camera target (player chest) ---
        Vector3 target = player.position + Vector3.up * 1.5f;

        // --- Ideal camera position ---
        Vector3 desiredPos = target + rotation * offset;

        // --- SphereCast for walls ---
        Vector3 dir = (desiredPos - target).normalized;
        float desiredDistance = offset.magnitude;

        RaycastHit hit;
        if (Physics.SphereCast(target, cameraRadius, dir, out hit, offset.magnitude, collisionMask, QueryTriggerInteraction.Ignore))
        {
            desiredDistance = Mathf.Max(hit.distance - wallOffset, minDistance);
        }
        else
        {
            desiredDistance = maxDistance;
        }

        // --- Smooth distance ---
        currentDistance = Mathf.SmoothDamp(currentDistance, desiredDistance, ref distanceVelocity, smoothTime);

        // --- Final camera position ---
        transform.position = target + dir * currentDistance;

        // --- Look at player ---
        transform.LookAt(target);
    }
}
