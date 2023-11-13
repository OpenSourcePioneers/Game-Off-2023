using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMachine
{
    private PlayerState lastState;
    public PlayerState curState {get; set;}
    public void SetState(PlayerState _state)
    {
        curState = _state;
        curState.EnterState();
        curState.TransitionCheck();
    }

    public void ChangeState(PlayerState _state)
    {
        if(_state == curState)
            return;
        curState.ExitState();
        lastState = curState;
        SetState(_state);
    }
}
