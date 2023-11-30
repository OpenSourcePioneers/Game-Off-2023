using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bodyslam", menuName = "Behaviours/Attack/Bodyslam", order = 0)]
public class Bodyslam : AttackSOBase
{
    [SerializeField] private AnimationCurve forceCurve;
    [SerializeField] private float damage;
    [SerializeField] private float damageRadius;
    [SerializeField] private float concussionAmount;

    Collider enemyCollider;
    float time;
    float refTime;
    float force;

    public override void Initialize(Enemy _enemy, GameObject _gameObject)
    {
        base.Initialize(_enemy, _gameObject);
        enemy.DebugCircle(damageRadius, Color.black);
        enemyCollider = enemy.GetComponent<Collider>();
    }
    public override void DoEnterState()
    {
        if(enemy.disToPlayer > damageRadius * 2)
        {
            int ind = enemy.attackInd;
            if(ind == enemy.instAttackBase.Count && ind != 0)
                ind--;
            else
                ind++;
            enemy.attack.SetInst(ind);
        }
        base.DoEnterState();
        refTime = time = 0f;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();

        time += Time.deltaTime;
        force = forceCurve.Evaluate(time);
        if(time > 0.5f && enemy.GroundCheck())
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
            foreach(Collider collider in colliders)
                DoCollisionCheck(enemyCollider, collider);
            DoTransitionCheck();
        }
    }
    public override void DoFixedFrameUpdate()
    {
        BodyslamOnGround();
    }
    public override void DoExitState()
    {
        base.DoExitState();
    }
    public override void DoTransitionCheck()
    {
        base.DoTransitionCheck();
        enemy.machine.ChangeState(enemy.combat);
    }

   public override void DoCollisionCheck(Collider thisCollider, Collider otherCollider)
    {
        base.DoCollisionCheck(thisCollider, otherCollider);
        if(otherCollider.transform == transform)
            return;
        IDamageable iDamageable;
        if(otherCollider.TryGetComponent<IDamageable>(out iDamageable))
        {
            iDamageable.Damage(damage, otherCollider.ClosestPoint(transform.position));
            iDamageable.Concussion(concussionAmount);
        }
    }

    private void BodyslamOnGround()
    {
        if(enemyRb.velocity.sqrMagnitude < force * force)
        {
            enemyRb.AddForce(transform.up * force * Universe.forceMult * 
                Time.fixedDeltaTime, ForceMode.Force);
        }
    }
}
