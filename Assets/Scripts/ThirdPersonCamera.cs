using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 8, -8);
    public float followSpeed = 10f;
    public float mouseSensitivity = 3f;

    public float yaw = 0f;
    public float pitch = 30f; // fixed top-down angle

    void Start()
    {
        yaw = player.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Mouse horizontal rotation
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = player.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
