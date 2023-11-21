using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FollowAndMelee", menuName = "Behaviours/Attack/SoldierOnly/FollowAndMelee", order = 0)]
public class FollowAndMelee : AttackSOBase
{
    [SerializeField] private AnimationCurve flightCurve;
    [SerializeField] private float maxHeight;
    [SerializeField] private float flightSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private float biteDash;
    [SerializeField] private float exitTime;
    [SerializeField] private int maxHits;

    Soldier soldier;
    int timesHited;
    float refTime;
    float refTimeForFlight;
    float defSpeed;

    public override void DoEnterState()
    {
        base.DoEnterState();
        if(soldier == null)
            soldier = enemy.GetComponent<Soldier>();
        head.AssignBehaviour(this);
        enemy.speed = flightSpeed;

        canAttack = false;
        refTimeForFlight = refTime = 0f;
        timesHited = 0;
        enemy.DebugCircle(attackRange, Color.green);
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        if(Timeout(exitTime) || timesHited >= maxHits)
        {
            //ExitConditions
            DoTransitionCheck();
        }
        if(enemy.disToPlayer < attackRange)
        {
            canAttack = true;
        }
        else if(canAttack)
        {
            if(soldier.flightStarted)
                soldier.EndFlight(ref refTimeForFlight);
            if(enemy.AimAtPlayer(0.1f, ref refTime, true))
            {
                enemyRb.AddForce(transform.forward.normalized * biteDash * 
                    Universe.forceMult * Time.deltaTime, ForceMode.Impulse);
                canAttack = false;
            }
        }
        else
        {
            enemy.RotateToTarget(player.position);
            enemy.vec = transform.forward;

            if(transform.position.y + 0.1f < maxHeight)
            {
                soldier.StartFlight(flightCurve, ref refTimeForFlight);
            }
        }
    }
    public override void DoFixedFrameUpdate()
    {
        if(!canAttack)
            FollowTarget();
    }
    public override void DoExitState()
    {
        base.DoExitState();
        enemy.lockedAtTarget = false;
        enemy.speed = enemy.defSpeed;
    }
    public override void DoTransitionCheck()
    {
        base.DoTransitionCheck();
        if(soldier.flightStarted)
            soldier.EndFlight(ref refTimeForFlight);
        enemy.machine.ChangeState(enemy.combat);
    }

    public override void DoCollisionCheck(Collider collider)
    {
        base.DoCollisionCheck(collider);
        //Attack animations here
        IDamageable iDamageable;
        if(collider.TryGetComponent<IDamageable>(out iDamageable))
        {
            iDamageable.Damage(damage, collider.ClosestPoint(transform.position));
            timesHited++;
        }
    }

    private void FollowTarget()
    {
        soldier.DoFlight(maxHeight, 0);
        enemy.Move(true);
    }
}
