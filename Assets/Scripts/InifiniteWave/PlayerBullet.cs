using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private bool _isFired = false;
    private float _speed;
    private float _damage;
    private float _range;
    private Vector2 _direction;
    private Vector3 _startPosition;

    private float _rotationSpeed = 720f;
    
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _hitRadius = 0.1f;

    // Shared buffer for non-alloc physics hits
    private static readonly Collider2D[] _hitResults = new Collider2D[16];
    
    public void Fire(Vector2 direction, float speed, float damage, float range)
    {
        _isFired = true;
        _speed = speed;
        _damage = damage;
        _range = range;
        _direction = direction.normalized;
        _startPosition = transform.position;
        gameObject.SetActive(true);
    }
    
    private void Update()
    {
        if (!_isFired) return;
        
        transform.RotateAround(transform.position, Vector3.forward, _rotationSpeed * -Time.deltaTime);

        // Move bullet
        Vector3 delta = _direction * (_speed * Time.deltaTime);
        transform.position += delta;

        // Range check
        if ((transform.position - _startPosition).sqrMagnitude > _range * _range)
        {
            Disable();
            return;
        }

        TryHitEnemy();
    }

    private void TryHitEnemy()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = _enemyLayer;
        contactFilter.useLayerMask = true;
        int count = Physics2D.OverlapCircle(transform.position, _hitRadius, contactFilter, _hitResults);

        if (count <= 0)
            return;
        
        Enemy bestEnemy = null;
        float bestSqrDist = float.MaxValue;
        Vector3 bulletPos = transform.position;

        IReadOnlyList<Enemy> alive = LevelManager.Instance?.AliveEnemies;

        for (int i = 0; i < count; i++)
        {
            Collider2D col = _hitResults[i];
            if (!col) continue;

            Enemy enemy = col.GetComponent<Enemy>();
            if (!enemy) continue;

            // (Optional) ensure this enemy is in AliveEnemies list
            if (alive != null && !((List<Enemy>)alive).Contains(enemy))
                continue;

            float sqrDist = (enemy.transform.position - bulletPos).sqrMagnitude;
            if (sqrDist < bestSqrDist)
            {
                bestSqrDist = sqrDist;
                bestEnemy = enemy;
            }
        }

        if (bestEnemy != null)
        {
            bestEnemy.TakeDamage(_damage);
            Disable(); // remove bullet on hit (no pierce)
        }
    }

    private void Disable()
    {
        _isFired = false;
        Player.Instance.ReturnBullet(this);
        gameObject.SetActive(false); // or return to pool
    }
}
