using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : Air
{
    float time;
    float force;

    public Jump(Player _player, StateMachine _machine) : base(_player, _machine)
    {
        
    }

    public override void EnterState()
    {
        time = 0f;
        force = 2f; 
    }

    public override void FrameUpdate()
    {
        time += Time.deltaTime;
        force = player.jumpCurve.Evaluate(time);
    }
    public override void FixedFrameUpdate()
    {
        TransitionCheck();
        player.MoveInDir(force, player.transform.up);
    }

    public override void TransitionCheck()
    {
        if(player.IsShiftClicked())
        {
            machine.ChangeState(player.dash);
        }
        else if(force < 2f)
            machine.ChangeState(player.inAir);
    }
}
