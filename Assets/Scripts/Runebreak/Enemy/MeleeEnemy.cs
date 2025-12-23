using UnityEngine;

public class MeleeEnemy : Enemy
{

    protected override bool CanAttack()
    {
        var targetPos = Target.transform.position;
        var dist = targetPos - transform.position;
        return dist.sqrMagnitude <= _attackRangeSqr;
    }

    protected override void Attack()
    {
        base.Attack();
        EventBus.Publish(new EnemyAttackEvent(this, AttackType.Melee, DamageType.Blunt, _damage));
    }
}
