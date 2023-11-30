using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWanderer", menuName = "Behaviours/Wander/RandomWanderer", 
    order = 0)]


public class RandomWanderer : WanderSOBase
{
    [SerializeField] private float wanderRange;
    [SerializeField] private float minWait;
    [SerializeField] private float maxWait;
    bool sent;

    public override void DoEnterState()
    {
        base.DoEnterState();
        sent = true;
        enemy.CallAfterTime(0f, SearchForTarget);
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        enemy.CheckForWayPoints();
        if(enemy.finishedPath && !sent)
        {
            sent = true;
            enemy.CallAfterTime(Random.Range(minWait, maxWait), SearchForTarget);
        }
    }
    public override void DoFixedFrameUpdate()
    {
        base.DoFixedFrameUpdate();
        enemy.Move();
    }
    public override void DoExitState()
    {
        base.DoExitState();
    }
    public override void DoTransitionCheck()
    {
        base.DoTransitionCheck();
    }

    private Vector3 GetRandomTarget()
    {
        Vector2 rand = Random.insideUnitCircle.normalized;
        Vector3 target = (new Vector3(rand.x, 0f, rand.y)  * wanderRange) + transform.position;
        if(TwoD_Grid.NodeFromWorldPosition(target).walkable)
            return target;
        else
            return GetRandomTarget();
    }

    private void SearchForTarget(bool success)
    {
        sent = !success;
        enemy.PushPathRequest(GetRandomTarget());
    }
}
