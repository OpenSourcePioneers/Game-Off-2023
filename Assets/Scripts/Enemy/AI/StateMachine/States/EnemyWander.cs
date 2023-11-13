using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWander : EnemyState
{
    public EnemyWander(Enemy _enemy, EnemyMachine _machine) : base(_enemy, _machine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.instWanderBase.DoEnterState();
    }
    public override void ExitState()
    {
        base.ExitState();
        enemy.instWanderBase.DoExitState();
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.instWanderBase.DoFrameUpdate();
    }
    public override void FixedFrameUpdate()
    {
        base.FixedFrameUpdate();
        enemy.instWanderBase.DoFixedFrameUpdate();
    }
    public override void TransitionCheck()
    {
        base.TransitionCheck();
        enemy.instWanderBase.DoTransitionCheck();
    }
}
