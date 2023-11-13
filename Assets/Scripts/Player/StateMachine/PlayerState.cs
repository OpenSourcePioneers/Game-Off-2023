using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerMachine machine;
    public PlayerState(Player _player, PlayerMachine _machine)
    {
        this.player = _player;
        this.machine = _machine;
    }

    public virtual void EnterState() {}
    public virtual void FrameUpdate() {}
    public virtual void FixedFrameUpdate() {}
    public virtual void ExitState() {}
    public virtual void TransitionCheck() {}
}