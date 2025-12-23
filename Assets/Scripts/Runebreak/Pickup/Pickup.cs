using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Transform _target;
    private float _collectRadiusSqr;

    private int _amount;

    private float _currentSpeed;
    private float _maxFlySpeed;
    private float _acceleration = 10f;
    
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

    public void Init(int amount)
    {
        _amount = amount;
    }

    public void BeginFlyTo(Transform target, float collectRadius, float startSpeed)
    {
        _currentSpeed = startSpeed;
        _maxFlySpeed = startSpeed * 3f;
        _target = target;
        _collectRadiusSqr = collectRadius * collectRadius;
        IsFlying = true;
    }

    private void Update()
    {
        if (!IsFlying || !_target) 
        {
            return;
        }

        Vector3 pos = transform.position;
        Vector3 toTarget = _target.position - pos;
        float distSq = toTarget.sqrMagnitude;
        
        if (distSq <= _collectRadiusSqr)
        {
            Collect();
            return;
        }
        
        _currentSpeed += _acceleration * Time.deltaTime;
        _currentSpeed = Mathf.Min(_currentSpeed, _maxFlySpeed);
        
        Vector3 step = toTarget.normalized * _currentSpeed * Time.deltaTime;
        transform.position = pos + step;
    }

    private void Collect()
    {
        EventBus.Publish(new PickupEvent(PickupType.Coin, _amount));

        IsFlying = false;
        gameObject.SetActive(false);
    }
}
