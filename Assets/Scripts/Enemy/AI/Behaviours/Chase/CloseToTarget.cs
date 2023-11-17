using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CloseToTarget", menuName = "Behaviours/Chase/CloseToTarget", 
    order = 0)]
    
public class CloseToTarget : ChaseSOBase
{
    [SerializeField] private float minWait;
    [SerializeField] private float maxWait;
    bool sent;

    public override void DoEnterState()
    {
        base.DoEnterState();

        enemy.PushPathRequest(GetRandomPosNearPlayer());
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        DoTransitionCheck();
        enemy.CheckForWayPoints();
        if((player.transform.position - target).magnitude > 2 * enemy.chaseRad && !sent)
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
        if(enemy.finishedPath || enemy.disToPlayer < enemy.chaseRad/1.4f)
            enemy.machine.ChangeState(enemy.combat);
    }

    private Vector3 GetRandomPosNearPlayer()
    {
        Vector2 rand = Random.insideUnitCircle.normalized;
        target = (new Vector3(rand.x, 0f, rand.y)  * enemy.chaseRad) 
            + player.transform.position;

        if(TwoD_Grid.NodeFromWorldPosition(target).walkable)
            return target;
            
        else
            return GetRandomPosNearPlayer();
    }

    private void SearchForTarget(bool success)
    {
        sent = !success;
        enemy.PushPathRequest(GetRandomPosNearPlayer());
    }
}
