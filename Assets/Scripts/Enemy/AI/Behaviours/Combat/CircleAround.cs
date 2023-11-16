using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "CircleAround", menuName = "Behaviours/Combat/CircleAround", 
    order = 0)]
public class CircleAround : CombatSOBase
{
    [SerializeField] private float cirRange;
    [SerializeField] private float minWait;
    [SerializeField] private float maxWait;
    bool sent;
    int count;
    float time;

    public override void DoEnterState()
    {
        base.DoEnterState();
        time = 0f;
        sent = false;
        enemy.PushPathRequest(GetRandomPosNearPlayer());
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        time += Time.deltaTime;
        DoTransitionCheck();
        enemy.CheckForWayPoints();
        if(!sent && enemy.finishedPath)
        {
            sent = true;
            enemy.CallAfterTime(Random.Range(minWait, maxWait), SearchForTarget);
        }
        else
        {
            enemy.RotateToTarget(player.transform.position);
            enemy.lockedAtTarget = true;
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
        enemy.lockedAtTarget = false;
    }
    public override void DoTransitionCheck()
    {
        base.DoTransitionCheck();
        if(time > Random.Range(attackTime, attackTime + 2f))
        {
            enemy.ResetPath();
            time = 0f;
            sent = true;
            enemy.machine.ChangeState(enemy.attack);
        }
    }

    private Vector3 GetRandomPosNearPlayer()
    {
        if(count > 10)
        {
            count = 0;
            return transform.position;
        }
        count++;
        Vector3 playerPos = player.transform.position;
        Vector2 rand = Random.insideUnitCircle.normalized;
        target = (new Vector3(rand.x, 0f, rand.y)  * cirRange)
            + playerPos;
        Ray ray1 = new Ray(target, (playerPos - target).normalized);
        if(TwoD_Grid.NodeFromWorldPosition(target).walkable && 
            !Physics.Raycast(ray1, cirRange, enemy.obstacle))
        {
            count = 0;
            return target;
        }
        else
            return GetRandomPosNearPlayer();
    }

    private void SearchForTarget(bool success)
    {
        sent = !success;
        enemy.PushPathRequest(GetRandomPosNearPlayer());
    }
}
