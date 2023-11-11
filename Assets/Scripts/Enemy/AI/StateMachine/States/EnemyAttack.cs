using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyState
{
    public EnemyAttack(Enemy _enemy, EnemyMachine _machine) : base(_enemy, _machine)
    {

    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.instAttackBase.DoEnterState();
    }
    public override void ExitState()
    {
        base.ExitState();
        enemy.instAttackBase.DoExitState();
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.instAttackBase.DoFrameUpdate();
    }
    public override void FixedFrameUpdate()
    {
        base.FixedFrameUpdate();
        enemy.instAttackBase.DoFixedFrameUpdate();
    }
    public override void TransitionCheck()
    {
        base.TransitionCheck();
        enemy.instAttackBase.DoTransitionCheck();
    }
}
