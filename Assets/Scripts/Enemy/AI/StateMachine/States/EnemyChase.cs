using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : EnemyState
{
    public EnemyChase(Enemy _enemy, EnemyMachine _machine) : base(_enemy, _machine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.instChaseBase.DoEnterState();
    }
    public override void ExitState()
    {
        base.ExitState();
        enemy.instChaseBase.DoExitState();
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.instChaseBase.DoFrameUpdate();
    }
    public override void FixedFrameUpdate()
    {
        base.FixedFrameUpdate();
        enemy.instChaseBase.DoFixedFrameUpdate();
    }
    public override void TransitionCheck()
    {
        base.TransitionCheck();
        enemy.instChaseBase.DoTransitionCheck();
    }
}
