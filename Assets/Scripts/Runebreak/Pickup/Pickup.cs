using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] float _baseFlySpeed = 12f;
    [SerializeField] float _maxFlySpeed = 40f;
    [SerializeField] float _maxSpeedDistance = 8f; // how far before we hit max speed

    private Transform _target;
    private float _collectRadiusSqr;

    public bool IsFlying { get; private set; }
    public Vector3 Position => transform.position;

    private void OnEnable()
    {
        PickupManager.Instance.Register(this);
        IsFlying = false;
        _target = null;
    }

    private void OnDisable()
    {
        PickupManager.Instance.Unregister(this);
    }

    public void BeginFlyTo(Transform target, float collectRadius)
    {
        _target = target;
        _collectRadiusSqr = collectRadius * collectRadius;
        IsFlying = true;
    }

    private void Update()
    {
        if (!IsFlying || !_target) return;

        Vector3 pos = transform.position;
        Vector3 toTarget = _target.position - pos;
        float dist = toTarget.magnitude;

        // If we reached collect radius, collect
        if (dist * dist <= _collectRadiusSqr)
        {
            Collect();
            return;
        }

        // Speed scales with distance so far-away pickups zoom in faster
        float t = Mathf.InverseLerp(0f, _maxSpeedDistance, dist);
        float speed = Mathf.Lerp(_baseFlySpeed, _maxFlySpeed, t);

        Vector3 step = toTarget.normalized * speed * Time.deltaTime;
        transform.position = pos + step;
    }

    private void Collect()
    {
        EventBus.Publish(new PickupEvent(PickupType.Coin, 1));

        IsFlying = false;
        gameObject.SetActive(false); // or return to pool explicitly
    }
}
