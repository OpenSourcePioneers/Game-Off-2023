using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSOBase : ScriptableObject 
{
    protected Enemy enemy;
    protected GameObject gameObject;
    protected Transform transform;
    protected Transform player;
    protected Rigidbody enemyRb;
    protected bool canAttack;
    [HideInInspector] public CollisionCheck head;
    [HideInInspector] public Transform shootTrans;

    float curTime;
    public virtual void Initialize(Enemy _enemy, GameObject _gameObject)
    {
        enemy = _enemy;
        gameObject = _gameObject;
        transform = gameObject.transform;
        player = GameObject.Find("Player").GetComponent<Transform>();
        enemyRb = enemy.GetComponent<Rigidbody>();
    }

    public virtual void DoEnterState() 
    {
        enemy.WalkState = false;
        canAttack = false;
        curTime = 0f;
        enemy.ResetPath();
    }
    public virtual void DoFrameUpdate() {}
    public virtual void DoFixedFrameUpdate() {}
    public virtual void DoExitState()
    {
        canAttack = false; 
        curTime = 0f;
        enemy.WalkState = true;
        enemy.ResetPath(); 
    }
    public virtual void DoTransitionCheck() {}
    public virtual void DoCollisionCheck(Collider collider) {}
    protected bool Timeout(float maxTime = 10f)
    {
        curTime += Time.deltaTime;
        if(curTime > maxTime)
        {
            return true;
        }
        return false;
    }
}