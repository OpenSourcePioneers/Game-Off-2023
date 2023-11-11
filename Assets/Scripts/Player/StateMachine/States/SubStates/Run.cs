using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : Grounded
{
    public Run(Player _player, PlayerMachine _machine) : base(_player, _machine)
    {

    }
    
    public override void EnterState()
    {
        base.EnterState();
    }
    public override void ExitState()
    {
        base.ExitState();
    }
    public override void FixedFrameUpdate()
    {
        TransitionCheck();
        player.Move(player.rSpeed);
    }

    public override void TransitionCheck()
    {
        base.TransitionCheck();
        if(!player.IsShiftHold())
        {
            if(player.IsMoving())
                machine.ChangeState(player.walk);
            else
                machine.ChangeState(player.idle);
        }
    }
}
