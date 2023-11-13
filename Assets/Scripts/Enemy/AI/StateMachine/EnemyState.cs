using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected Enemy enemy;
    protected EnemyMachine machine;
    public EnemyState(Enemy _enemy, EnemyMachine _machine)
    {
        this.enemy = _enemy;
        this.machine = _machine;
    }

    public virtual void EnterState() {}
    public virtual void FrameUpdate() {}
    public virtual void FixedFrameUpdate() {}
    public virtual void ExitState() {}
    public virtual void TransitionCheck() {}
}