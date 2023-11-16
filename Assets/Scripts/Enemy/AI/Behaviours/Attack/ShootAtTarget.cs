using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootAtTarget", menuName = "Behaviours/Attack/ShootAtTarget", 
    order = 0)]
public class ShootAtTarget : AttackSOBase
{
    [SerializeField] private CollisionCheck projectile;
    [SerializeField] private float damage;
    [SerializeField] private float projHeight;
    [SerializeField] private float aimTime;

    Rigidbody projRb;
    float curTime;
    CollisionCheck collisionCheck;

    public override void DoEnterState()
    {
        base.DoEnterState();
        curTime = 0f;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();

        if(!canAttack)
        {
            canAttack = enemy.AimAtPlayer(aimTime, ref curTime);
        }else
        {
            ShootProjectile();
            DoTransitionCheck();
        }
    }
    public override void DoFixedFrameUpdate()
    {

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

    public override void DoCollisionCheck(Collider collider)
    {
        base.DoCollisionCheck(collider);

        IDamageable iDamageable;
        if(collider.TryGetComponent<IDamageable>(out iDamageable))
        {
            iDamageable.Damage(damage, collider.ClosestPoint(transform.position));
        }

        Destroy(collisionCheck.gameObject);
    }

    private void ShootProjectile()
    {
        projRb = Instantiate(projectile, shootTrans.position, transform.rotation).GetComponent<Rigidbody>();
        projRb.velocity = Predict();
        collisionCheck = projRb.gameObject.GetComponent<CollisionCheck>();
        collisionCheck.enemy = enemy;
        collisionCheck.AssignBehaviour(this);
        Destroy(collisionCheck.gameObject, 5f);
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
