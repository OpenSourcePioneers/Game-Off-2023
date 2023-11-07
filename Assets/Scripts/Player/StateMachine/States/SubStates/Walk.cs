using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : Grounded
{
    public Walk(Player _player, StateMachine _machine) : base(_player, _machine)
    {
        
    }

    public override void FixedFrameUpdate()
    {
        TransitionCheck();
        player.Move(player.wSpeed);
    }

    public override void TransitionCheck()
    {
        base.TransitionCheck();
        if(!player.IsMoving())
            machine.ChangeState(player.idle);
        else if(player.IsShiftHold())
            machine.ChangeState(player.run);
    }
}
