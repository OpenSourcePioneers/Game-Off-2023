using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAir : State
{
    public InAir(Player _player, StateMachine _machine) : base(_player, _machine)
    {
        
    }

    public override void TransitionCheck()
    {
        base.TransitionCheck();
        if(player.IsShiftClicked())
        {
            machine.ChangeState(player.dash);
        }
        else if(player.IsLMouseClick())
        {
            machine.ChangeState(player.attack);
        }
    }
}
