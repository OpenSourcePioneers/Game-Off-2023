using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : State
{
    public Grounded(Player _player, StateMachine _machine) : base(_player, _machine)
    {

    }


    public override void TransitionCheck()
    {
        base.TransitionCheck();
        if(player.IsShiftClicked())
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
