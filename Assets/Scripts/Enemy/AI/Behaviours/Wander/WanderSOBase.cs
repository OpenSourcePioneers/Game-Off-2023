using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderSOBase : ScriptableObject 
{
    protected Enemy enemy;
    protected GameObject gameObject;
    protected Transform transform;
    protected Player player;
    [SerializeField] private float viewRad;
    
    public virtual void Initialize(Enemy _enemy, GameObject _gameObject)
    {
        enemy = _enemy;
        gameObject = _gameObject;
        transform = gameObject.transform;
        player = GameObject.Find("Player").GetComponent<Player>();
        enemy.DebugCircle(viewRad, Color.white);
    }

    public virtual void DoEnterState() {}
    public virtual void DoFrameUpdate() 
    {
        DoTransitionCheck();
    }
    public virtual void DoFixedFrameUpdate() {}
    public virtual void DoExitState() {enemy.ResetPath();}
    public virtual void DoTransitionCheck()
    {
        if(enemy.disToPlayer < viewRad)
            enemy.machine.ChangeState(enemy.chase);
    }
}
