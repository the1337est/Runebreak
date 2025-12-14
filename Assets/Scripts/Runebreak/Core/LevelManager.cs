using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
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
    
    

    public bool IsWaveActive { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _background.drawMode = SpriteDrawMode.Tiled;
        _background.size = new Vector2(_worldSize.x+1, _worldSize.y+1);
    }
    
    private void OnEnable()
    {
        EventBus.Subscribe<GameSceneEnterEvent>(HandleGameSceneEnter);
        EventBus.Subscribe<NextWaveClickEvent>(HandleNextWaveClick);
        EventBus.Subscribe<EnemySpawnEvent>(HandleEnemySpawn);
        EventBus.Subscribe<EnemyDeathEvent>(HandleEnemyDeath);
        
        EventBus.Subscribe<PlayerDeathEvent>(HandlePlayerDeath);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<GameSceneEnterEvent>(HandleGameSceneEnter);
        EventBus.Unsubscribe<NextWaveClickEvent>(HandleNextWaveClick);
        EventBus.Unsubscribe<EnemySpawnEvent>(HandleEnemySpawn);
        EventBus.Unsubscribe<EnemyDeathEvent>(HandleEnemyDeath);
        
        EventBus.Unsubscribe<PlayerDeathEvent>(HandlePlayerDeath);
    }
    
    private void HandleGameSceneEnter(GameSceneEnterEvent obj)
    {
        //generate level idk
        _waveIndex = -1; //reset wave index
        EventBus.Publish(new GameStartEvent());
        StartNextWave();
    }
    
    private void HandleNextWaveClick(NextWaveClickEvent obj)
    {
        StartNextWave();
    }
    
    private void HandlePlayerDeath(PlayerDeathEvent eventData)
    {
        //save wave details if any
        IsWaveActive = false;
    }

    private void StartNextWave()
    {
        _waveIndex++;
        if (_waveIndex >= _waves.Count)
        {
            Debug.Log("End of waves reached!");
            return;
        }

        _currentWave = _waves[_waveIndex];
        LoadWave(_currentWave);
        IsWaveActive = true;
        WaveTick();
        _nextSpawnTime = Time.time + _initialDelay;
        EventBus.Publish(new WaveStartEvent(_waveIndex+1));
    }

    private void LoadWave(WaveData wave)
    {
        _timer = wave.Time;
        _waveEnemiesCache = wave.EnemyProbilities.Select(e => (e.Enemy, e.Probability)).ToList();
    }
    
    private async Task WaveTick()
    {
        if (!IsWaveActive) return;
        EventBus.Publish(new TimerUpdateEvent(_timer));
        await Task.Delay(1000);
        _timer -= 1;
        if (_timer >= 1)
        {
            WaveTick();
        }
        else
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        //remove all alive enemies
        for (int i = _aliveEnemies.Count - 1; i >= 0; i--)
        {
            Destroy(_aliveEnemies[i].gameObject);
        }
        _aliveEnemies.Clear();
        //return player to the center
        IsWaveActive = false;
        
        EventBus.Publish(new TimerUpdateEvent(_timer));
        EventBus.Publish(new WaveEndEvent());
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
            SpawnEnemy();
            _nextSpawnTime = Time.time + _spawnInterval;
        }
    }

    private void Update()
    {
        ProcessWaveUpdate();
    }

    private CancellationTokenSource _cts;
    
    private async void SpawnEnemy()
    {
        var count = GameMath.GetBiasedInt(_currentWave.MinSpawnCount, _currentWave.MaxSpawnCount, _currentWave.SpawnCountBias);
        _cts = new CancellationTokenSource();   
        for (int i = 0; i < count; i++)
        {
            try
            {
                var prefab = GameMath.GetWeightedPrefab(_waveEnemiesCache);
                var position = new Vector2(Random.Range(-_worldSize.x / 2f, _worldSize.x / 2f),
                    Random.Range(-_worldSize.y / 2f, _worldSize.y / 2f));

                _cts.Token.ThrowIfCancellationRequested();
                if (!this || !gameObject.activeInHierarchy) return;

                var spawn = Instantiate(_spawnPrefab, position, Quaternion.identity);
                spawn.Init(prefab, position);
                var delay = Random.Range(50, 250);
                await Task.Delay(delay, _cts.Token);
            }
            catch (TaskCanceledException)
            {
                
            }
        }
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
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
        _cts?.Cancel();
    }
}