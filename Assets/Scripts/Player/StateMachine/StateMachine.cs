using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private State lastState;
    public State curState {get; set;}
    public void SetState(State _state)
    {
        curState = _state;
        curState.EnterState();
        curState.TransitionCheck();
    }

    public void ChangeState(State _state)
    {
        if(lastState == curState)
            return;
        curState.ExitState();
        lastState = curState;
        SetState(_state);
    }
}
