using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyState
{
    AttackSOBase thisAttack;
    public EnemyAttack(Enemy _enemy, EnemyMachine _machine) : base(_enemy, _machine)
    {

    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.attackInd = Random.Range(0, enemy.instAttackBase.Count);
        thisAttack = enemy.instAttackBase[enemy.attackInd];
        thisAttack.DoEnterState();
    }
    public override void ExitState()
    {
        base.ExitState();
        thisAttack.DoExitState();
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();
        thisAttack.DoFrameUpdate();
    }
    public override void FixedFrameUpdate()
    {
        base.FixedFrameUpdate();
        thisAttack.DoFixedFrameUpdate();
    }
    public override void TransitionCheck()
    {
        base.TransitionCheck();
        thisAttack.DoTransitionCheck();
    }
}
