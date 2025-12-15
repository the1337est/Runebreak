using System;
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
        SpawnEnemyAsync();
    }

    private async void SpawnEnemyAsync()
    {
        try
        {
            _cts = new CancellationTokenSource();
            await Task.Delay(1200, _cts.Token);

            _cts.Token.ThrowIfCancellationRequested();
            if (!this || !gameObject.activeInHierarchy) return;

            var e = Instantiate(_enemy, _position, Quaternion.identity);
            EventBus.Publish(new EnemySpawnEvent(e));
            Destroy(gameObject);
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
