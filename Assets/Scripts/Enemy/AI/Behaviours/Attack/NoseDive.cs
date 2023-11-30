using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoseDive", menuName = "Behaviours/Attack/SoldierOnly/NoseDive", 
    order = 0)]
public class NoseDive : AttackSOBase
{
    [SerializeField] private AnimationCurve forceCurve;
    [SerializeField] private AnimationCurve flightCurve;
    [SerializeField] private float maxHeight;
    [SerializeField] private float damage;
    [SerializeField] private float aimTime;
    [SerializeField] private float concussionAmount;

    Soldier soldier;
    List<Collider> damagedCollider = new List<Collider>();
    bool completedFlight;
    float time;
    float refTime;
    float refTimeForFlight;
    float force;

    public override void DoEnterState()
    {
        base.DoEnterState();
        if(soldier == null)
            soldier = enemy.GetComponent<Soldier>();
        head.AssignBehaviour(this);
        damagedCollider = new List<Collider>();
        refTimeForFlight = refTime = time = 0f;
        force = 2f;
        completedFlight = false;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        DoTransitionCheck();
        if(transform.position.y + 0.1f < maxHeight && !completedFlight)
        {
            soldier.StartFlight(flightCurve, ref refTimeForFlight);
        }
        else
        {
            if(!canAttack)
            {
                if(completedFlight == false)
                {   
                    completedFlight = true;
                }

                canAttack = enemy.AimAtPlayer(aimTime, ref refTime, true);
            }
            else
            {
                if(soldier.flightStarted)
                    soldier.EndFlight(ref refTimeForFlight);
                time += Time.deltaTime;
                force = forceCurve.Evaluate(time);
            }
        }
    }
    public override void DoFixedFrameUpdate()
    {
        if(!completedFlight)
            soldier.DoFlight(maxHeight);
        if(canAttack)
            NoseDiveTheTarget();
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
            if(soldier.flightStarted)
                soldier.EndFlight(ref refTimeForFlight);
            enemy.machine.ChangeState(enemy.combat);
        }
    }

    public override void DoCollisionCheck(Collider thisCollider, Collider otherCollider)
    {
        base.DoCollisionCheck(thisCollider, otherCollider);
        IDamageable iDamageable;
        if(canAttack && otherCollider.TryGetComponent<IDamageable>(out iDamageable))
        {
            foreach (Collider c in damagedCollider)
                return;
            
            damagedCollider.Add(otherCollider);
            iDamageable.Damage(damage, otherCollider.ClosestPoint(transform.position));
            iDamageable.Concussion(concussionAmount);
        }
    }

    private void NoseDiveTheTarget()
    {
        if(enemyRb.velocity.sqrMagnitude < force * force)
        {
            enemyRb.AddForce(transform.forward * force * Universe.forceMult * 
                Time.fixedDeltaTime, ForceMode.Force);
        }
    }
}