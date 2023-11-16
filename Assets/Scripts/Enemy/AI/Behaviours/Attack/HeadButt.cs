using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeadButt", menuName = "Behaviours/Attack/HeadButt", order = 0)]
public class HeadButt : AttackSOBase
{
    [SerializeField] private float damage;
    [SerializeField] private AnimationCurve forceCurve;
    [SerializeField] private float aimTime;

    List<Collider> damagedCollider = new List<Collider>();
    float time;
    float curTime;
    float force;

    public override void DoEnterState()
    {
        base.DoEnterState();
        head.AssignBehaviour(this);
        damagedCollider = new List<Collider>();
        curTime = time = 0f;
        force = 2f;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        DoTransitionCheck();

        if(!canAttack)
            canAttack = enemy.AimAtPlayer(aimTime, ref curTime);
        else
        {
            time += Time.deltaTime;
            force = forceCurve.Evaluate(time);
        }
    }
    public override void DoFixedFrameUpdate()
    {
        if(canAttack)
            HeadButtTheTarget();
    }
    public override void DoExitState()
    {
        base.DoExitState();

    }
    public override void DoTransitionCheck()
    {
        base.DoTransitionCheck();
        if(force < 2f)
        {
            enemy.machine.ChangeState(enemy.combat);
        }
    }

    public override void DoCollisionCheck(Collider collider)
    {
        base.DoCollisionCheck(collider);

        IDamageable iDamageable;
        if(canAttack && collider.TryGetComponent<IDamageable>(out iDamageable))
        {
            foreach (Collider c in damagedCollider)
            {
                return;
            }
            damagedCollider.Add(collider);
            iDamageable.Damage(damage, collider.ClosestPoint(transform.position));
        }
    }

    private void HeadButtTheTarget()
    {
        if(enemyRb.velocity.sqrMagnitude < force * force)
        {
            enemyRb.AddForce(transform.forward * force * Universe.forceMult * 
                Time.fixedDeltaTime, ForceMode.Force);
        }
    }
}
