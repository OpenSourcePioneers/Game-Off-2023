using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMachine
{
    private EnemyState lastState;
    public EnemyState curState {get; set;}
    public void SetState(EnemyState _state)
    {
        curState = _state;
        curState.EnterState();
    }

    public void ChangeState(EnemyState _state)
    {
        if(_state == curState)
            return;
        curState.ExitState();
        lastState = curState;
        SetState(_state);
    }
}