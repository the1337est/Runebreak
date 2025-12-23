using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    private Enemy _enemy;
    private Vector2 _position;

    private CancellationTokenSource _cts;

    public void Init(Enemy enemy, Vector2 position)
    {
        _enemy = enemy;
        _position = position;
        StartCoroutine(SpawnAfterDelay(1.2f));
    }

    private void OnEnable()
    {
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<WaveEndEvent>(HandleWaveEnd);
    }

    private void HandleWaveEnd(WaveEndEvent obj)
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private IEnumerator SpawnAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (gameObject == null || !gameObject.activeInHierarchy) yield break;
        var e = Instantiate(_enemy, _position, Quaternion.identity);
        EventBus.Publish(new EnemySpawnEvent(e));
        Destroy(gameObject);
    }
}
