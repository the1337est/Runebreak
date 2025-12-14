using UnityEngine;

public class EnemyDeathEvent : IGameEvent
{
    /// <summary>
    /// Only available until the end of death animation
    /// </summary>
    public Enemy Enemy;
    public Vector2 Position;
    public int Coins;

    public EnemyDeathEvent(Enemy enemy, Vector2 position, int coins = 1)
    {
        Enemy = enemy;
        Position = position;
        Coins = coins;
    }
}

public class EnemySpawnEvent : IGameEvent
{
    public Enemy Enemy;

    public EnemySpawnEvent(Enemy enemy)
    {
        Enemy = enemy;
    }

    public override string ToString()
    {
        return $"{Enemy.name} spawned";
    }
}

public class EnemyAttackEvent : IGameEvent
{
    public Enemy Enemy;
    public AttackType AttackType;
    public DamageType DamageType;
    public float Damage;

    public EnemyAttackEvent(Enemy enemy, AttackType attackType, DamageType damageType, float damage)
    {
        Enemy = enemy;
        AttackType = attackType;
        DamageType = damageType;
        Damage = damage;
    }
    
    public override string ToString()
    {
        return $"{Enemy.name} did {Damage} {DamageType} {AttackType} damage";
    }
}
