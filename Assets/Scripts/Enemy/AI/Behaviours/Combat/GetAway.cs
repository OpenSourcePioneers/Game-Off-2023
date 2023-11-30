using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetAway", menuName = "Behaviours/Combat/GetAway", order = 0)]
public class GetAway : CombatSOBase
{
    [SerializeField] private float cirRange;
    [SerializeField] private float criticalRange;
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
        enemy.PushPathRequest(GetRandomPosFarFromPlayer());
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        time += Time.deltaTime;
        DoTransitionCheck();
        enemy.CheckForWayPoints();
        if(enemy.disToPlayer < criticalRange && enemy.finishedPath)
        {
            enemy.ResetPath();

            //Calculate the optimal location to get away
            float sDis = float.PositiveInfinity;
            Vector3 sVec = transform.position;
            for(int i = 0; i < 10; i++)
            {
                Vector3 nVec = GetRandomPosFarFromPlayer() - transform.position;
                if(nVec.sqrMagnitude < sDis)
                {
                    sVec = nVec + transform.position;
                    sDis = nVec.sqrMagnitude;
                }
            }
            enemy.PushPathRequest(sVec);
            sent = false;
        }
        else if(!sent && enemy.finishedPath)
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

    private Vector3 GetRandomPosFarFromPlayer()
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
            return GetRandomPosFarFromPlayer();
    }

    private void SearchForTarget(bool success)
    {
        sent = !success;
        enemy.PushPathRequest(GetRandomPosFarFromPlayer());
    }
}
