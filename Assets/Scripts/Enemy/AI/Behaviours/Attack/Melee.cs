using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "Behaviours/Attack/Melee")]
public class Melee : AttackSOBase
{
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private float biteDash;
    Vector3 target;
    float curTime;
    bool damaged;
    Color defColor;
    MeshRenderer renderer;

    public override void DoEnterState()
    {
        base.DoEnterState();
        canAttack = false;
        curTime = 0f;
        target = player.transform.position;
        enemy.PushPathRequest(target);
        head.AssignBehaviour(this);
        enemy.DebugCircle(attackRange, Color.green);
        renderer =  enemy.GetComponentInChildren<MeshRenderer>();
        defColor = renderer.material.color;
        renderer.material.color = Color.red;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        if(canAttack)
        {
            if(enemy.AimAtPlayer(0.1f, ref curTime))
            {
                enemy.ResetPath();
                enemyRb.AddForce((transform.forward + transform.up).normalized * biteDash * 
                    Universe.forceMult * Time.deltaTime, ForceMode.Impulse);
                DoTransitionCheck();
            }

        }
        else if(enemy.disToPlayer < attackRange)
        {
            canAttack = true;
        }
        else if((player.transform.position - target).sqrMagnitude > attackRange * attackRange)
        {
            target = player.transform.position;
            enemy.PushPathRequest(target);
        }else
        {
            enemy.RotateToTarget(player.transform.position);
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
        renderer.material.color = defColor;
        enemy.lockedAtTarget = false;
    }
    public override void DoTransitionCheck()
    {
        base.DoTransitionCheck();
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
            damaged = true;
        }
    }
}