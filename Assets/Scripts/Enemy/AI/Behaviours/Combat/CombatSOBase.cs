using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSOBase : ScriptableObject 
{
    protected Enemy enemy;
    protected GameObject gameObject;
    protected Transform transform;
    protected Player player;
    protected Vector3 target;

    [SerializeField] protected float attackTime;

    public virtual void Initialize(Enemy _enemy, GameObject _gameObject)
    {
        enemy = _enemy;
        gameObject = _gameObject;
        transform = gameObject.transform;
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public virtual void DoEnterState() {}
    public virtual void DoFrameUpdate() {}
    public virtual void DoFixedFrameUpdate() {}
    public virtual void DoExitState() {enemy.ResetPath();}
    public virtual void DoTransitionCheck()
    {
        if(enemy.disToPlayer > enemy.chaseRad)
            enemy.machine.ChangeState(enemy.chase);
    }
}

