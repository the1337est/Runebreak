using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 6f;      // how fast camera catches up
    [SerializeField] private Vector2 offset;              // screen offset for framing

    [Header("Bounds (World Space)")]
    public float minX, maxX;
    public float minY, maxY;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerPositionSnapEvent>(HandlePlayerPositionSnap);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerPositionSnapEvent>(HandlePlayerPositionSnap);
    }

    private void HandlePlayerPositionSnap(PlayerPositionSnapEvent eventData)
    {
        transform.position += eventData.Offset;
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

        transform.position = smoothPos;
    }
}