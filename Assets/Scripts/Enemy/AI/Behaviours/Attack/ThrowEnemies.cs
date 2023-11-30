using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowEnemies", menuName = "Behaviours/Attack/ThrowEnemies", 
    order = 0)]
public class ThrowEnemies : AttackSOBase, IShooter
{
    [SerializeField] private List<Enemy> minions;
    [SerializeField] private float projHeight;
    [SerializeField] private float aimTime;
    [field: SerializeField] public int maxShots{get; set;}
    [field: SerializeField] public float delay{get; set;}

    Rigidbody projRb;
    float refTime;
    public float timeSinceLastShot{get; set;}
    public int shotsFired{get; set;}
    TwoD_Grid gridForChildren;

    public override void Initialize(Enemy _enemy, GameObject _gameObject)
    {
        base.Initialize(_enemy, _gameObject);
        gridForChildren = enemy.GetComponent<Queen>().gridForMinions;
    }
    public override void DoEnterState()
    {
        base.DoEnterState();
        timeSinceLastShot = refTime = 0f;
        shotsFired = 0;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        if(Timeout())
        {
            DoTransitionCheck();
        }
        timeSinceLastShot += Time.deltaTime;
        canAttack = enemy.AimAtPlayer(aimTime, ref refTime);

        if(timeSinceLastShot > delay)
            ShootMinions();
        if(shotsFired >= maxShots)
            DoTransitionCheck();
        
    }
    public override void DoFixedFrameUpdate()
    {
        base.DoFixedFrameUpdate();
    }
    public override void DoExitState()
    {
        base.DoExitState();
    }
    public override void DoTransitionCheck()
    {
        base.DoTransitionCheck();
        enemy.machine.ChangeState(enemy.combat);
    }
    public override void DoCollisionCheck(Collider thisCollider, Collider otherCollider)
    {
        base.DoCollisionCheck(thisCollider, otherCollider);
    }

    private void ShootMinions()
    {
        projRb = Instantiate(minions[Random.Range(0, minions.Count)], shootTrans.position, transform.rotation).GetComponent<Rigidbody>();
        projRb.velocity = Predict() * 2;
        Enemy enemy = projRb.GetComponent<Enemy>();
        enemy.grid = gridForChildren;
        shotsFired++;
        timeSinceLastShot = 0f;
    }

    private Vector3 Predict()
    {
        float g = -Physics.gravity.magnitude;
        Vector3 s = player.transform.position - shootTrans.position;
        float h = s.y + projHeight;
        if(h < projHeight)
            h = projHeight;
        Vector3 disInXZ = new Vector3(s.x, 0f, s.z);
        Vector3 velY = Vector3.up * Mathf.Sqrt(-2 * g * h);
        Vector3 velXZ = disInXZ / (Mathf.Sqrt(-2 * h/g) + Mathf.Sqrt(2 *(s.y - h)/g));
        return velY + velXZ;
    }
}
