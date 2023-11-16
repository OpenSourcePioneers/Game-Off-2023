using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : PlayerState
{
    public Grounded(Player _player, PlayerMachine _machine) : base(_player, _machine)
    {

    }
    public override void EnterState()
    {
        base.EnterState();
        player.grounded = true;
    }
    public override void ExitState()
    {
        base.ExitState();
        player.grounded = false;
    }
    public override void TransitionCheck()
    {
        base.TransitionCheck();
        if(player.IsShiftClicked() && Time.realtimeSinceStartupAsDouble - 
            player.dash.leaveTime > player.dashDelay)
        {
            machine.ChangeState(player.dash);
        }
        else if(!player.OnGround())
        {
            machine.ChangeState(player.airCtrl);
        }
        else if(player.IsSpacePressed())
        {
            machine.ChangeState(player.jump);
        }
        else if(player.IsLMouseClick())
        {
            machine.ChangeState(player.attack);
        }
    }
}
