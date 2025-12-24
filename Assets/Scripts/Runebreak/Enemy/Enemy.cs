using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Player Target { get; private set; }

    [Header("Stats")]
    [SerializeField] protected float _maxHealth = 10;

    [Header("Reward")]
    [SerializeField] protected int _coinsOnDeath = 1;

    [SerializeField] protected float _moveSpeed = 4f;
    [SerializeField] protected float _dashSpeed;
    [SerializeField] protected float _separation = 0.25f;
    [SerializeField] protected float _attackInterval = 1f;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _damage;

    
    [SerializeField] protected float _dashInterval;
    [SerializeField] protected float _dashDuration;

    private float _dashEndTime;
    
    private bool _isDashing = false;
    
    private float _nextDashTime;
    
    private float _currentHealth;
    private float _nextAttackTime;

    protected float _attackRangeSqr;
    
    private EnemyHealthWidget _healthWidget;

    public bool IsAlive;

    private bool _waveActive;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    private bool _movementLock = false;
    public void Init()
    {
        _currentHealth = _maxHealth;
        Target = Player.Instance;
        _nextAttackTime = Time.time + _attackInterval;
        _nextDashTime = Time.time + Random.Range(0f, 3f) + _dashInterval;
        IsAlive = true;
        _attackRangeSqr = _attackRange * _attackRange;
        _movementLock = true;
        StartCoroutine(UnblockMovement());
    }

    private IEnumerator UnblockMovement()
    {
        yield return new WaitForSeconds(0.5f);
        _movementLock = false;
    }

    private void Awake()
    {
        _healthWidget = GetComponentInChildren<EnemyHealthWidget>();
        if (_healthWidget != null)
        {
            _healthWidget.SetHealthNormalized(1f);
        }

        if (_currentHealth <= 0)
            _currentHealth = _maxHealth;
    }

    private void Update()
    {
        DashUpdate();
        MovementUpdate();
        AttackUpdate();
    }

    private void DashUpdate()
    {
        if (!_isDashing) return;
        if (!(Time.time >= _dashEndTime)) return;
        _isDashing = false;
        _nextDashTime = Time.time + Random.Range(0f, 1f) + _dashInterval;
    }

    private void MovementUpdate()
    {
        if (!CanMove()) return;
        var maxDelta = _moveSpeed * Time.deltaTime;
        if (!_isDashing && Time.time >= _nextDashTime)
        {
            _isDashing = true;
            _dashEndTime = Time.time + _dashDuration;
            maxDelta = _dashSpeed * Time.deltaTime;
        }
        transform.position = Vector3.MoveTowards(
            transform.position + GetSeparation(),
            Target.transform.position,
            maxDelta
        );
        var p = transform.position;
        transform.position = new Vector3(LevelManager.Instance.GetClampedX(p.x), LevelManager.Instance.GetClampedY(p.y), p.z);
    }

    private bool CanMove()
    {
        if (!IsAlive) return false;
        if (!LevelManager.Instance.IsWaveActive) return false;
        if (_movementLock) return false;
        return true;
    }
    
    private void AttackUpdate()
    {
        if (!IsAlive) return;
        if (!(Time.time >= _nextAttackTime)) return;
        if (!CanAttack()) return;
        _spriteRenderer.DOColor(Color.magenta, 0.25f).OnComplete(() =>
        {
            if (gameObject != null)
            {
                Attack();
            }
            if (_spriteRenderer != null)
            {
                _spriteRenderer.DOColor(Color.white, 0.15f);
            }
        });
        UpdateNextAttackTime();
    }

    protected virtual bool CanAttack()
    {
        return true;
    }

    //to be called when an attack is processed from Enemy implementation
    protected void UpdateNextAttackTime()
    {
        _nextAttackTime = Time.time + _attackInterval;
    }

    protected virtual void Attack()
    {
        
    }

    private Vector3 GetSeparation()
    {
        Vector3 push = Vector3.zero;

        foreach (var other in LevelManager.Instance.AliveEnemies)
        {
            if (other == this) continue;
            float d = Vector3.Distance(transform.position, other.transform.position);

            if (d < _separation)
                push += (transform.position - other.transform.position).normalized * (_separation - d);
        }

        return push;
    }
    
    // Call this from your damage system / bullets / player attacks
    public void TakeDamage(float amount)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= amount;
        if (_healthWidget != null)
        {
            _healthWidget.SetHealthNormalized(_currentHealth/_maxHealth);
        }
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //publish event
        EventBus.Publish(new EnemyDeathEvent(this, transform.position, _coinsOnDeath));
        PlayDeathSequence();
    }

    private void PlayDeathSequence()
    {
        IsAlive = false;
        var scale = transform.localScale;
        _spriteRenderer.DOColor(Color.red, 0.1f).SetEase(Ease.InOutBack);
        
        transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InOutBack).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }
}