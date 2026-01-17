using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 8, -8);
    public float followSpeed = 10f;
    public float mouseSensitivity = 3f;

    [Header("Vertical limits")]
    public float minPitch = 10f;  // lowest vertical angle
    public float maxPitch = 25f;  // highest vertical angle

    public float yaw = 0f;
    public float pitch = 15f; // starting angle

    void Start()
    {
        yaw = player.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // --- Mouse rotation ---
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        // Small vertical movement with clamping
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity; // invert if needed
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // --- Calculate rotation and position ---
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = player.position + rotation * offset;

        // --- Smooth follow ---
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // --- Look at player ---
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }

    public float Yaw => yaw;
}

