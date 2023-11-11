using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : EnemyState
{
    public EnemyCombat(Enemy _enemy, EnemyMachine _machine) : base(_enemy, _machine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.instCombatBase.DoEnterState();
    }
    public override void ExitState()
    {
        base.ExitState();
        enemy.instCombatBase.DoExitState();
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.instCombatBase.DoFrameUpdate();
    }
    public override void FixedFrameUpdate()
    {
        base.FixedFrameUpdate();
        enemy.instCombatBase.DoFixedFrameUpdate();
    }
    public override void TransitionCheck()
    {
        base.TransitionCheck();
        enemy.instCombatBase.DoTransitionCheck();
    }
}
