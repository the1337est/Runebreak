using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-90)]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Enemy Spawning")]
    [SerializeField] private Enemy _enemy1Prefab;
    [SerializeField] private Enemy _enemy2Prefab;
    [SerializeField] private float _initialDelay = 1f;
    [SerializeField] private float _spawnInterval = 1.2f;
    [SerializeField] private int _maxAliveEnemies = 200;
    
    [Header("Wave Settings")] 
    [SerializeField] private List<WaveData> _waves;

    [SerializeField] private Spawn _spawnPrefab;
    
    [Header("Refs")] 
    [SerializeField] private SpriteRenderer _background;
    
    private readonly List<Enemy> _aliveEnemies = new();

    public Vector2 WorldSize => _worldSize;
    
    [SerializeField] private Vector2 _worldSize;
    
    public List<Enemy> AliveEnemies => _aliveEnemies;
    private float _nextSpawnTime;

    private WaveData _currentWave;
    private IList<(Enemy, float)> _waveEnemiesCache = new List<(Enemy, float)>();
    private int _waveIndex = -1;
    private int _timer;

    [SerializeField] private StatsSO _playerBaseStats;
    
    
    public bool IsWaveActive { get; private set; }

    [FormerlySerializedAs("_floor")] [SerializeField] private CurrentFloor currentFloor;
    [SerializeField] private NextLevelFloor _nextFloor;    
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _background.drawMode = SpriteDrawMode.Tiled;
        _background.size = new Vector2(_worldSize.x, _worldSize.y);
    }
    
    private void OnEnable()
    {
        EventBus.Subscribe<GameSceneEnterEvent>(HandleGameSceneEnter);
        EventBus.Subscribe<NextWaveClickEvent>(HandleNextWaveClick);
        EventBus.Subscribe<EnemySpawnEvent>(HandleEnemySpawn);
        EventBus.Subscribe<EnemyDeathEvent>(HandleEnemyDeath);
        
        EventBus.Subscribe<PlayerDeathEvent>(HandlePlayerDeath);
        EventBus.Subscribe<LevelSwitchOverEvent>(HandleLevelSwitch);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<GameSceneEnterEvent>(HandleGameSceneEnter);
        EventBus.Unsubscribe<NextWaveClickEvent>(HandleNextWaveClick);
        EventBus.Unsubscribe<EnemySpawnEvent>(HandleEnemySpawn);
        EventBus.Unsubscribe<EnemyDeathEvent>(HandleEnemyDeath);
        
        EventBus.Unsubscribe<PlayerDeathEvent>(HandlePlayerDeath);
        EventBus.Unsubscribe<LevelSwitchOverEvent>(HandleLevelSwitch);
    }
    
    private void HandleGameSceneEnter(GameSceneEnterEvent obj)
    {
        //generate level idk
        _waveIndex = -1; //reset wave index
        Player.Instance.Init(_playerBaseStats);
        EventBus.Publish(new GameStartEvent());
        StartNextWave();
        Debug.Log("Game Scene Entered, player has initialized");
    }
    
    private void HandleNextWaveClick(NextWaveClickEvent obj)
    {
        StartNextWave();
    }

    private void HandleLevelSwitch(LevelSwitchOverEvent eventData)
    {
        Player.Instance.SnapPositionOffset(Vector2.left * _worldSize.x);
        StartNextWave();
    } 

    private void HandlePlayerDeath(PlayerDeathEvent eventData)
    {
        //save wave details if any
        IsWaveActive = false;
    }

    private WaveData GetNextWave()
    {
        WaveData wave = null;
        if (_waveIndex < _waves.Count - 1)
        {
            wave = _waves[_waveIndex + 1];
        }

        return wave;
    }

    private void StartNextWave()
    {
        _waveIndex++;
        if (_waveIndex >= _waves.Count)
        {
            EventBus.Publish(new GameEndEvent());
            return;
        }

        _currentWave = _waves[_waveIndex];
        currentFloor.Initialize(_currentWave);
        _nextFloor.gameObject.SetActive(false);
        _nextFloor.Initialize(GetNextWave());
        LoadWave(_currentWave);
        IsWaveActive = true;
        StartCoroutine(WaveTick());
        _nextSpawnTime = Time.time + _initialDelay;
        EventBus.Publish(new WaveStartEvent(_waveIndex+1));
    }

    private void LoadWave(WaveData wave)
    {
        _timer = wave.Time;
        _waveEnemiesCache = wave.EnemyProbilities.Select(e => (e.Enemy, e.Probability)).ToList();
    }
    
    private IEnumerator WaveTick()
    {
        if (!IsWaveActive) yield break;
        while (_timer >= 1)
        {
            _timer -= 1;
            EventBus.Publish(new TimerUpdateEvent(_timer));
            yield return new WaitForSeconds(1f);
        }
        ProcessEnd();
    }

    private void ProcessEnd()
    {
        if (_waveIndex >= _waves.Count-1)
        {
            IsWaveActive = false;
            EventBus.Publish(new GameEndEvent());
        }
        else
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        StopCoroutine(SpawnEnemyRandomDelay());
        IsWaveActive = false;
        EnableNextLevel();
        ClearAllEnemies();
        EventBus.Publish(new TimerUpdateEvent(_timer));
        EventBus.Publish(new WaveEndEvent());
    }

    private void ClearAllEnemies()
    {
        if (_aliveEnemies == null) return;
        for (int i = _aliveEnemies.Count - 1; i >= 0; i--)
        {
            Destroy(_aliveEnemies[i].gameObject);
        }
        _aliveEnemies.Clear();
    }

    private void EnableNextLevel()
    {
        _nextFloor.gameObject.SetActive(true);
    }

    private void ProcessWaveUpdate()
    {
        if (!IsWaveActive) return;
        if (_enemy1Prefab == null || _enemy2Prefab == null)
            return;

        if (_aliveEnemies.Count >= _maxAliveEnemies)
            return;

        if (Time.time >= _nextSpawnTime)
        {
            StartCoroutine(SpawnEnemyRandomDelay());
            _nextSpawnTime = Time.time + _spawnInterval;
        }
    }

    private void Update()
    {
        ProcessWaveUpdate();
        #if UNITY_EDITOR
        CheatsUpdate();
        #endif   
    }

    private void CheatsUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _timer = 0;
            if(IsWaveActive) EndWave();
        }
    }

    private IEnumerator SpawnEnemyRandomDelay()
    {
        var count = GameMath.GetBiasedInt(_currentWave.MinSpawnCount, _currentWave.MaxSpawnCount, _currentWave.SpawnCountBias);
        for (int i = 0; i < count; i++)
        {
            if(gameObject == null) yield break;
            var prefab = GameMath.GetWeightedPrefab(_waveEnemiesCache);
            var position = new Vector2(Random.Range(-_worldSize.x / 2f, _worldSize.x / 2f),
                Random.Range(-_worldSize.y / 2f, _worldSize.y / 2f));
            var spawn = Instantiate(_spawnPrefab, position, Quaternion.identity);
            spawn.Init(prefab, position);
            var delay = Random.Range(0.02f, 0.1f);
            yield return new WaitForSeconds(delay);
            
        }

        yield return null;
    }


    private void HandleEnemySpawn(EnemySpawnEvent eventData)
    {
        var e = eventData.Enemy;
        e.transform.SetParent(transform);
        e.Init();
        _aliveEnemies.Add(e);
    }

    private void HandleEnemyDeath(EnemyDeathEvent eventData)
    {
        if (eventData.Enemy == null) return;
        _aliveEnemies.Remove(eventData.Enemy);
    }
    
    public float GetClampedY(float y)
    {
        return Mathf.Clamp(y, -_worldSize.y/2f, _worldSize.y/2f);
    }
    
    public float GetClampedX(float x)
    {
        return Mathf.Clamp(x, -_worldSize.x/2f, _worldSize.x/2f);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}