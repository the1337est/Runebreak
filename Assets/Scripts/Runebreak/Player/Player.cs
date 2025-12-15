using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class Player : MonoBehaviour
{
    private InputActions _inputActions;

    [SerializeField] private float _moveSpeed;

    private Vector2 _movement;
    private bool _lookingRight = true;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    public Vector2 Position => transform.position;

    public static Player Instance;

    private List<PlayerBullet> _bullets;
    private List<PlayerBullet> _availableBullets;
    
    [SerializeField] private PlayerBullet _bulletPrefab;
    [SerializeField] private Transform _bulletOrigin;

    [SerializeField] private float _range = 5f;
    [SerializeField] private float _attackInterval = 1f;
    [SerializeField] private float _projectileSpeed = 5f;
    
    [Header("Health")]
    [SerializeField] private float _maxHealth = 10f;
    [SerializeField] private float _currentHealth;
    
    private float _nextAttackTime = 0f;

    public PlayerState State { get; private set; }
    
    private Stats _stats;
    public Stats Stats => _stats;

    private void Awake()
    {
        Instance = this;
        _inputActions = new InputActions();
        _inputActions.Enable();
        _bullets = new List<PlayerBullet>();
        _availableBullets = new List<PlayerBullet>();
        State = PlayerState.Menu;
        _stats = new Stats();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
        
        EventBus.Subscribe<EnemyAttackEvent>(HandleEnemyAttack);
        EventBus.Subscribe<PickupEvent>(HandlePickupEvent);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Unsubscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Unsubscribe<EnemyAttackEvent>(HandleEnemyAttack);
        EventBus.Unsubscribe<PickupEvent>(HandlePickupEvent);
    }

    private void Update()
    {
        if(!CanControl()) return;
        MovementUpdate();
        AttackUpdate();
    }

    private void AttackUpdate()
    {
        if (Time.time >= _nextAttackTime)
        {
            Attack();
        }
    }
    
    private void HandleWaveStart(WaveStartEvent eventData)
    {
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        State = PlayerState.Idle;
        _currentHealth = _maxHealth;
        _stats.Set(StatType.MaxHP, _maxHealth);
        _stats.Set(StatType.HP, _currentHealth);
        _spriteRenderer.color = Color.white;
        transform.position = Vector2.zero;
    }

    private void HandleWaveEnd(WaveEndEvent eventData)
    {
        State = PlayerState.Menu;
    }
    
    private void HandleEnemyAttack(EnemyAttackEvent eventData)
    {
        //TODO: do immunity, dodge and armour calculations here
        TakeDamage(eventData.Damage);
    }

    public void SetPlayerState(PlayerState state)
    {
        State = state;
    }

    private bool CanControl()
    {
        return State == PlayerState.Idle || State == PlayerState.Moving;
    }

    private void Attack()
    {
        var bestDistance = float.MaxValue;
        Enemy bestEnemy = null;
        foreach (var e in LevelManager.Instance.AliveEnemies)
        {
            if(!e.IsAlive) continue;
            var dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist > _range) continue;
            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestEnemy = e;
            }
        }

        if (bestEnemy != null)
        {
            _nextAttackTime = Time.time + _attackInterval;
            PlayerBullet bullet = null;
            if (_availableBullets.Count > 0)
            {
                bullet = _availableBullets.First();
                _availableBullets.Remove(bullet);
            }
            else
            {
                bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
                _bullets.Add(bullet);
            }
            bullet.transform.position = _bulletOrigin.position;
            bullet.Fire(bestEnemy.transform.position - _bulletOrigin.position, _projectileSpeed, 10f, _range);
        }
    }

    private void MovementUpdate()
    {
        if(!_inputActions.Player.Move.IsInProgress()) return;
        
        var input = _inputActions.Player.Move.ReadValue<Vector2>();
        _movement = input.normalized;

        // if (_lookingRight && _movement.x < 0f)
        // {
        //     _lookingRight = false;
        //     _spriteRenderer.flipX = true;
        // }
        // else if(!_lookingRight && _movement.x > 0f)
        // {
        //     _lookingRight = true;
        //     _spriteRenderer.flipX = false;
        // }
        
        transform.Translate(_movement * _moveSpeed * Time.deltaTime);
        var p = transform.position;
        transform.position = new Vector3(LevelManager.Instance.GetClampedX(p.x), LevelManager.Instance.GetClampedY(p.y), p.z);
    }

    public void ReturnBullet(PlayerBullet bullet)
    {
        _availableBullets.Add(bullet);
    }
    
    private void HandlePickupEvent(PickupEvent eventData)
    {
        switch (eventData.PickupType)
        {
            case PickupType.Coin:
                AddCoin(eventData.BaseAmount);
                break;
            
            case PickupType.Health:
                AddHealth(eventData.BaseAmount);
                break;
            
            case PickupType.LootBox:
                AddLootBox(eventData.BaseAmount);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddCoin(int baseAmount)
    {
        var coins = _stats.Get(StatType.Coins);
        //add stat calculation here to baseAmount
        coins += baseAmount;
        _stats.Set(StatType.Coins, coins);
    }
    
    private void AddHealth(int baseAmount)
    {
        var hp = _stats.Get(StatType.HP);
        //add stat calculation here to baseAmount
        hp += baseAmount;
        _stats.Set(StatType.HP, hp);
    }
    
    private void AddLootBox(int baseAmount)
    {
        var boxes = _stats.Get(StatType.UnopenedBox);
        //add stat calculation here to baseAmount
        boxes += baseAmount;
        _stats.Set(StatType.UnopenedBox, boxes);
    }
    
    private void TakeDamage(float amount)
    {
        var hp = _stats.Get(StatType.HP);
        if (hp <= 0) return;

        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
        _stats.Set(StatType.HP, Mathf.Clamp(hp, 0, _stats.Get(StatType.MaxHP)));
    }

    private void Die()
    {
        _spriteRenderer.DOColor(Color.red, 0.2f);
        SetPlayerState(PlayerState.Dead);
        EventBus.Publish(new PlayerDeathEvent());
    }

    private void OnDestroy()
    {
        _inputActions.Disable();
    }
}

public enum PlayerState
{
    None,
    Menu,
    Idle,
    Moving,
    Dead
}
