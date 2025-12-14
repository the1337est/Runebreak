using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 6f;      // how fast camera catches up
    [SerializeField] private Vector2 offset;              // screen offset for framing

    [Header("Bounds (World Space)")]
    public bool useBounds = false;
    public float minX, maxX;
    public float minY, maxY;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (!target) return;

        // Desired camera position
        Vector3 desired = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        // Smooth follow
        Vector3 smoothPos = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);

        // Apply bounds if required
        if (useBounds)
            smoothPos = ClampToBounds(smoothPos);

        transform.position = smoothPos;
    }

    private Vector3 ClampToBounds(Vector3 camPos)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float clampedX = Mathf.Clamp(camPos.x, minX + camWidth, maxX - camWidth);
        float clampedY = Mathf.Clamp(camPos.y, minY + camHeight, maxY - camHeight);

        return new Vector3(clampedX, clampedY, camPos.z);
    }
}