using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeadButt", menuName = "Behaviours/Attack/HeadButt", order = 0)]
public class HeadButt : AttackSOBase
{
    [SerializeField] public float damage;
    [SerializeField] private AnimationCurve forceCurve;

    List<Collider> damagedCollider = new List<Collider>();
    float time;
    float focusTime;
    float force;

    public override void DoEnterState()
    {
        base.DoEnterState();
        damagedCollider = new List<Collider>();
        focusTime = time = 0f;
        force = 2f;
        canAttack = false;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        DoTransitionCheck();

        if(!canAttack)
            LookAtTarget();
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

        if(canAttack)
        {
            foreach (Collider c in damagedCollider)
            {
                if(c == collider)
                    return;
            }
            damagedCollider.Add(collider);
            collider.GetComponent<IDamageable>().Damage(damage);
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

    private void LookAtTarget()
    {
        Ray ray = new Ray(transform.position, transform.forward.normalized);
        if(!Physics.Raycast(ray, enemy.chaseRad * 2, enemy.playerMask))
        {
            //Rotate to look at player
            enemy.RotateToTarget(player.transform.position);
            focusTime = 0f;
        }
        else
        {
            focusTime += Time.deltaTime;
            if(focusTime > 0.5f)
                canAttack = true;
        }
    }
}
