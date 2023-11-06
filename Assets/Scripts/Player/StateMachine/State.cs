using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Player player;
    protected StateMachine machine;
    public State(Player _player, StateMachine _machine)
    {
        this.player = _player;
        this.machine = _machine;
    }

    public virtual void EnterState() {}
    public virtual void FrameUpdate() {}
    public virtual void FixedFrameUpdate() {}
    public virtual void ExitState() {}
    public virtual void TransitionCheck() {
        if(player.IsShiftClicked())
        {
            machine.ChangeState(player.dash);
        }
        else if(player.IsSpacePressed() && player.OnGround())
        {
            machine.ChangeState(player.jump);
        }
    }
}