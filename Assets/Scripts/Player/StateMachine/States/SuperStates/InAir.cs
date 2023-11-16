using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAir : PlayerState
{
    public InAir(Player _player, PlayerMachine _machine) : base(_player, _machine)
    {
        
    }

    public override void TransitionCheck()
    {
        base.TransitionCheck();
        if(player.IsShiftClicked() && Time.realtimeSinceStartupAsDouble - 
            player.dash.leaveTime > player.dashDelay)
        {
            machine.ChangeState(player.dash);
        }
        else if(player.IsLMouseClick())
        {
            machine.ChangeState(player.attack);
        }
    }
}
