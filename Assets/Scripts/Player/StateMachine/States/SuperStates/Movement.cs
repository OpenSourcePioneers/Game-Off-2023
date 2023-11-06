using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : State
{
    public Movement(Player _player, StateMachine _machine) : base(_player, _machine)
    {

    }


    public override void TransitionCheck()
    {
        base.TransitionCheck();
    }
}
