using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Movement
{
    float time;
    float force;

    public Dash(Player _player, StateMachine _machine) : base(_player, _machine)
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
        force = player.dashCurve.Evaluate(time);
    }
    public override void FixedFrameUpdate()
    {
        TransitionCheck();
        player.MoveInDir(force, player.transform.forward);
    }

    public override void TransitionCheck()
    {
        if(force < 2f)
            machine.ChangeState(player.idle);
    }
}
