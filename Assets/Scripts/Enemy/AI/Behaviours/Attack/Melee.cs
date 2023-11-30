using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "Behaviours/Attack/Melee")]
public class Melee : AttackSOBase
{
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private float biteDash;
    [SerializeField] private int maxHits = 1;

    Vector3 target;
    bool damaged;
    float refTime;
    int curHits;

    public override void DoEnterState()
    {
        base.DoEnterState();
        canAttack = false;
        refTime = 0f;
        curHits = 0;
        target = player.position;
        enemy.PushPathRequest(target);
        head.AssignBehaviour(this);
        enemy.DebugCircle(attackRange, Color.green);
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        if(Timeout() || curHits >= maxHits)
        {
            enemy.CallAfterTime(0.5f, WaitForTransition);
            return;
        }
        
        if(canAttack)
        {
            if(enemy.AimAtPlayer(0.1f, ref refTime))
            {
                enemy.ResetPath();
                enemyRb.AddForce(transform.forward.normalized * biteDash * 
                    Universe.forceMult * Time.deltaTime, ForceMode.Impulse);
                canAttack = false;
                curHits++;
            }
        }
        else if(enemy.disToPlayer < attackRange)
        {
            canAttack = true;
        }
        else if((player.position - target).magnitude > attackRange)
        {
            target = player.position;
            enemy.PushPathRequest(target);
        }else
        {
            enemy.RotateToTarget(player.position);
            enemy.lockedAtTarget = true;
            enemy.CheckForWayPoints();
        }

    }
    public override void DoFixedFrameUpdate()
    {
        if(!canAttack)
            enemy.Move();
    }
    public override void DoExitState()
    {
        base.DoExitState();
        enemy.lockedAtTarget = false;
    }
    public override void DoTransitionCheck()
    {
        base.DoTransitionCheck();
        enemy.machine.ChangeState(enemy.combat);
    }

    public override void DoCollisionCheck(Collider thisCollider, Collider otherCollider)
    {
        base.DoCollisionCheck(thisCollider, otherCollider);
        //Attack animations here
        IDamageable iDamageable;
        if(otherCollider.TryGetComponent<IDamageable>(out iDamageable))
        {
            iDamageable.Damage(damage, otherCollider.ClosestPoint(transform.position));
            damaged = true;
        }
    }

    private void WaitForTransition(bool success)
    {
        DoTransitionCheck();
    }
}