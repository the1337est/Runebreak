using System;
using System.Threading.Tasks;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    private Enemy _enemy;
    private Vector2 _position;

    public void Init(Enemy enemy, Vector2 position)
    {
        _enemy = enemy;
        _position = position;
        SpawnEnemyAsync();
    }

    private async void SpawnEnemyAsync()
    {
        await Task.Delay(1200);
        var e = Instantiate(_enemy, _position, Quaternion.identity);
        EventBus.Publish(new EnemySpawnEvent(e));
        Destroy(gameObject);
    }

}
