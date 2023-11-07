using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Combat
{
    public Attack(Player _player, StateMachine _machine) : base(_player, _machine)
    {

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        TransitionCheck();
    }

    public override void TransitionCheck()
    {
        base.TransitionCheck();
        if(!player.IsLMouseClick())
        {
            machine.ChangeState(player.idle);
        }
    }
}
