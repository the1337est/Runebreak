using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class Player : MonoBehaviour
{
    private InputActions _inputActions => GameManager.Instance.InputActions;
    
    private Vector2 _movement;
    private bool _lookingRight = true;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    public Vector2 Position => transform.position;

    public static Player Instance;

    private List<PlayerBullet> _bullets;
    private List<PlayerBullet> _availableBullets;
    
    [SerializeField] private PlayerBullet _bulletPrefab;
    [SerializeField] private Transform _bulletOrigin;
    
    [Header("Stats Cache")]
    [SerializeField] private float _range = 5f;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _attackInterval = 1f;
    [SerializeField] private float _projectileSpeed = 12f;
    [SerializeField] private float _hpRegen = 0f;
    
    private float _nextAttackTime = 0f;

    public PlayerState State { get; private set; }
    public Stats Stats { get; private set; }
    public Resources Resources { get; private set; }

    private StatsSO _baseStats;

    private bool _allowPastRightBorder;

    private Vector2 _worldSize;

    [SerializeField] private GameObject _healthWidgetWorld;
    
    private void Awake()
    {
        Instance = this;
        _bullets = new List<PlayerBullet>();
        _availableBullets = new List<PlayerBullet>();
        State = PlayerState.Menu;
        Stats = new Stats();
        Resources = new Resources();
    }

    public void SnapPositionOffset(Vector3 offset)
    {
        transform.position += offset;
        EventBus.Publish(new PlayerPositionSnapEvent(offset));
    }
    
    public void Init(StatsSO baseStats)
    {
        _baseStats = baseStats;
        _worldSize = LevelManager.Instance.WorldSize;
        InitStats(_baseStats);
        InitResources();
        EventBus.Publish(new PlayerSpawnEvent(this));
    }

    private void OnEnable()
    {
        EventBus.Subscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Subscribe<EnemyAttackEvent>(HandleEnemyAttack);
        EventBus.Subscribe<PickupEvent>(HandlePickupEvent);
        EventBus.Subscribe<ShopCoinsSpentEvent>(HandleShopCoinsSpent);
        EventBus.Subscribe<ShopBuyEvent>(HandleShopBuyEvent);
        
        EventBus.Subscribe<GameEndEvent>(HandleGameEnd);
        
        EventBus.Subscribe<PlayerGameValueChangeEvent<StatType>>(HandlePlayerStatChange);
        EventBus.Subscribe<PlayerGameValueRequestEvent<StatType>>(HandlePlayerStatRequest);
        EventBus.Subscribe<PlayerGameValueRequestEvent<ResourceType>>(HandlePlayerResourceRequest);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Unsubscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Unsubscribe<EnemyAttackEvent>(HandleEnemyAttack);
        EventBus.Unsubscribe<PickupEvent>(HandlePickupEvent);
        EventBus.Unsubscribe<ShopCoinsSpentEvent>(HandleShopCoinsSpent);
        EventBus.Unsubscribe<ShopBuyEvent>(HandleShopBuyEvent);
        
        EventBus.Unsubscribe<GameEndEvent>(HandleGameEnd);
        
        EventBus.Unsubscribe<PlayerGameValueChangeEvent<StatType>>(HandlePlayerStatChange);
        
        EventBus.Unsubscribe<PlayerGameValueRequestEvent<StatType>>(HandlePlayerStatRequest);
        EventBus.Unsubscribe<PlayerGameValueRequestEvent<ResourceType>>(HandlePlayerResourceRequest);
    }

    private void Update()
    {
        if(!CanControl()) return;
        MovementUpdate();
        AttackUpdate();
        RegenUpdate();
    }

    private void AttackUpdate()
    {
        if (Time.time >= _nextAttackTime)
        {
            Attack();
        }
    }
    
    private void RegenUpdate()
    {
        if (_hpRegen <= 0f) return;
        var hp = Resources.Get(ResourceType.HP);
        var maxHp = Stats.Get(StatType.MaxHP);
        hp += _hpRegen * Time.deltaTime;
        hp = Mathf.Clamp(hp, 0, maxHp);
        Resources.Set(ResourceType.HP, hp);
    }
    
    private void HandleWaveStart(WaveStartEvent eventData)
    {
        _allowPastRightBorder = false;
        State = PlayerState.Idle;
        Resources.Set(ResourceType.HP, Stats.Get(StatType.MaxHP));
    }

    private void HandlePlayerStatRequest(PlayerGameValueRequestEvent<StatType> eventData)
    {
        var value = Stats.Get(eventData.ValueType);
        var data = new PlayerGameValueChangeEvent<StatType>(new GameValueChange<StatType>(eventData.ValueType, value));
        EventBus.Publish(data);
    }

    private void HandlePlayerResourceRequest(PlayerGameValueRequestEvent<ResourceType> eventData)
    {
        var value = Resources.Get(eventData.ValueType);
        var data = new PlayerGameValueChangeEvent<ResourceType>(new GameValueChange<ResourceType>(eventData.ValueType, value));
        EventBus.Publish(data);
    }
    
    private void ResetPlayer()
    {
        State = PlayerState.Idle;
        _spriteRenderer.color = Color.white;
        transform.position = Vector2.zero;
        InitStats(_baseStats);
        InitResources();
    }
    
    private void InitStats(StatsSO baseStats)
    {
        Stats = new Stats();
        foreach (var item in baseStats.Stats)
        {
            Stats.Set(item.Stat, item.Value);
        }
    }

    private void InitResources()
    {
        Resources = new Resources();
        Resources.Set(ResourceType.Coins, 0);
        Resources.Set(ResourceType.HP, Stats.Get(StatType.MaxHP));
    }

    private void HandleWaveEnd(WaveEndEvent eventData)
    {
        State = PlayerState.Shopping;
        _allowPastRightBorder = true;
    }
    
    private void HandleGameEnd(GameEndEvent eventData)
    {
        State = PlayerState.None;
    }
    
    private void HandleEnemyAttack(EnemyAttackEvent eventData)
    {
        //TODO: do immunity, dodge and armour calculations here
        TakeDamage(eventData.Damage);
    }
    
    private void HandleShopCoinsSpent(ShopCoinsSpentEvent eventData)
    {
        var coins = Resources.Get(ResourceType.Coins);
        coins -= eventData.Coins;
        Resources.Set(ResourceType.Coins, coins);
    }

    private void HandleShopBuyEvent(ShopBuyEvent eventData)
    {
        var coins = Resources.Get(ResourceType.Coins);
        coins -= eventData.Item.BaseCost;
        Resources.Set(ResourceType.Coins, coins);
        ApplyUpgrade(eventData.Item);
    }
    
    private void HandlePlayerStatChange(PlayerGameValueChangeEvent<StatType> eventData)
    {
        switch (eventData.ValueType)
        {
            case StatType.Speed:
                _moveSpeed = eventData.Amount;
                _projectileSpeed = _moveSpeed * 2f;
                break;
            
            case StatType.Range:
                _range = eventData.Amount;
                break;
            
            case StatType.AttackSpeed:
                _attackInterval = 1f/eventData.Amount;
                break;
            
            case StatType.Damage:
                break;
            case StatType.Armour:
                break;
            case StatType.Dodge:
                break;
            case StatType.HPRegen:
                _hpRegen = eventData.Amount;
                break;
            case StatType.LifeSteal:
                break;
            case StatType.Collect:
                break;
            case StatType.PickupRange:
                break;
            case StatType.DamageOnCollect:
                break;
        }
    }

    private void ApplyUpgrade(UpgradeSO upgrade)
    {
        foreach (var change in upgrade.Changes)
        {
            ProcessChange(change);
        }
    }

    private void ProcessChange(StatChange statChange)
    {
        var value = Stats.Get(statChange.Stat);
        var baseValue = _baseStats.Stats.Find(f => f.Stat == statChange.Stat).Value;
        
        switch (statChange.Type)
        {
            case StatChangeType.Flat:
                Stats.Set(statChange.Stat, value + statChange.Amount);
                break;
            
            case StatChangeType.Percent:
                Stats.Set(statChange.Stat, value + baseValue * (statChange.Amount/100f));
                break;
            
            case StatChangeType.Multiply:
                Stats.Set(statChange.Stat, baseValue * statChange.Amount);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetPlayerState(PlayerState state)
    {
        State = state;
    }

    private bool CanControl()
    {
        return State == PlayerState.Idle || State == PlayerState.Moving || State == PlayerState.Shopping;
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

            var damage = Stats.Get(StatType.Damage);
            bullet.transform.position = _bulletOrigin.position;
            bullet.Fire(bestEnemy.transform.position - _bulletOrigin.position, _projectileSpeed, damage, _range);
            EventBus.Publish(new PlayerAttackEvent());
        }
    }

    private void MovementUpdate()
    {
        if(!_inputActions.Player.Move.IsInProgress()) return;
        
        var input = _inputActions.Player.Move.ReadValue<Vector2>();
        _movement = input.normalized;
        
        transform.Translate(_movement * _moveSpeed * Time.deltaTime);
        var p = transform.position;
        transform.position = new Vector3(GetClampedX(p.x), GetClampedY(p.y), p.z);
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
        var coins = Resources.Get(ResourceType.Coins);
        //add stat calculation here to baseAmount
        coins += baseAmount;
        Resources.Set(ResourceType.Coins, coins);
    }
    
    private void AddHealth(int baseAmount)
    {
        var hp = Resources.Get(ResourceType.HP);
        //add stat calculation here to baseAmount
        hp += baseAmount;
        Resources.Set(ResourceType.HP, hp);
    }
    
    private void AddLootBox(int baseAmount)
    {
        var boxes = Resources.Get(ResourceType.LootBox);
        //add stat calculation here to baseAmount
        boxes += baseAmount;
        Resources.Set(ResourceType.LootBox, boxes);
    }
    
    private void TakeDamage(float amount)
    {
        var hp = Resources.Get(ResourceType.HP);
        if (hp <= 0) return;

        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
        Resources.Set(ResourceType.HP, Mathf.Clamp(hp, 0, Stats.Get(StatType.MaxHP)));
        EventBus.Publish(new PlayerHitEvent());
    }

    private void Die()
    {
        _spriteRenderer.DOColor(Color.red, 0.2f);
        SetPlayerState(PlayerState.Dead);
        EventBus.Publish(new PlayerDeathEvent());
    }
    
    public float GetClampedY(float y)
    {
        return Mathf.Clamp(y, -_worldSize.y/2f, _worldSize.y/2f);
    }
    
    public float GetClampedX(float x)
    {
        var max = _allowPastRightBorder ? _worldSize.x * 1.5f : _worldSize.x/2f;
        return Mathf.Clamp(x, -_worldSize.x/2f, max);
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
    Dead,
    Shopping
}
