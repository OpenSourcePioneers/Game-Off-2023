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

    float time;
    float refTime;
    float force;

    public override void Initialize(Enemy _enemy, GameObject _gameObject)
    {
        base.Initialize(_enemy, _gameObject);
        enemy.DebugCircle(damageRadius, Color.black);
    }
    public override void DoEnterState()
    {
        base.DoEnterState();
        refTime = time = 0f;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();

        time += Time.deltaTime;
        force = forceCurve.Evaluate(time);
        if(time > 5f && enemy.GroundCheck())
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
            foreach(Collider collider in colliders)
                DoCollisionCheck(collider);
            DoTransitionCheck();
        }
    }
    public override void DoFixedFrameUpdate()
    {
        if(time > 0.5f)
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

    public override void DoCollisionCheck(Collider collider)
    {
        base.DoCollisionCheck(collider);
        if(collider.transform == transform)
            return;
        IDamageable iDamageable;
        if(collider.TryGetComponent<IDamageable>(out iDamageable))
        {
            iDamageable.Damage(damage, collider.ClosestPoint(transform.position));
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
