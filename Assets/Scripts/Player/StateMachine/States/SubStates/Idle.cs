using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : Grounded
{
    public Idle(Player _player, PlayerMachine _machine) : base(_player, _machine)
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
    
    public override void FrameUpdate() => TransitionCheck();

    public override void TransitionCheck()
    {
        base.TransitionCheck();
        
        if(player.IsMoving())
        {
            if(player.IsShiftHold())
                machine.ChangeState(player.run);
            else
                machine.ChangeState(player.walk);
        }
    }
}
