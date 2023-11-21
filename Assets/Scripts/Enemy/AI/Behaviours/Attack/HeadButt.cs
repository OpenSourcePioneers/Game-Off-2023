using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeadButt", menuName = "Behaviours/Attack/HeadButt", order = 0)]
public class HeadButt : AttackSOBase
{
    [SerializeField] private AnimationCurve forceCurve;
    [SerializeField] private float damage;
    [SerializeField] private float aimTime;
    [SerializeField] private float concussionAmount;
    [SerializeField] private float concussionWhenHitWall;
    
    List<Collider> damagedCollider = new List<Collider>();
    float time;
    float refTime;
    float force;

    public override void DoEnterState()
    {
        base.DoEnterState();
        head.AssignBehaviour(this);
        damagedCollider = new List<Collider>();
        refTime = time = 0f;
        force = 2f;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        DoTransitionCheck();

        if(!canAttack)
            canAttack = enemy.AimAtPlayer(aimTime, ref refTime);
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
        if(force < 2f || Timeout())
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
            iDamageable.Concussion(concussionAmount);
        }
        else if(canAttack && collider.gameObject.layer == enemy.obstacle)
            enemy.Concussion(concussionWhenHitWall);
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
