using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootInAnArea", menuName = "Behaviours/Attack/ShootInAnArea", 
    order = 0)]
public class ShootInAnArea : AttackSOBase, IShooter
{
    [SerializeField] private CollisionCheck projectile;
    [SerializeField] private float damage;
    [SerializeField] private float projHeight;
    [SerializeField] private float projShootRadius;
    [Range(0f, 100f)] [SerializeField] private float directPercentage = 10f;
    [field: SerializeField] public int maxShots{get; set;}
    [field: SerializeField] public float delay{get; set;}

    Rigidbody projRb;
    CollisionCheck collisionCheck;
    float refTime;
    public float timeSinceLastShot{get; set;}
    public int shotsFired{get; set;}

    public override void DoEnterState()
    {
        base.DoEnterState();
        timeSinceLastShot = refTime = 0f;
        shotsFired = 0;
    }
    public override void DoFrameUpdate()
    {
        base.DoFrameUpdate();
        if(Timeout(20f))
        {
            DoTransitionCheck();
        }
        timeSinceLastShot += Time.deltaTime;

        if(timeSinceLastShot > delay)
                ShootProjectile();
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

        IDamageable iDamageable;
        if(otherCollider.TryGetComponent<IDamageable>(out iDamageable))
        {
            iDamageable.Damage(damage, otherCollider.ClosestPoint(transform.position));
        }

        Destroy(thisCollider);
    }

    private void ShootProjectile()
    {
        projRb = Instantiate(projectile, shootTrans.position, transform.rotation).GetComponent<Rigidbody>();
        projRb.velocity = Predict();
        CollisionCheck collisionCheck;
        collisionCheck = projRb.gameObject.GetComponent<CollisionCheck>();
        collisionCheck.enemy = enemy;
        collisionCheck.AssignBehaviour(this);
        Destroy(collisionCheck.gameObject, 5f);
        shotsFired++;
        timeSinceLastShot = 0f;
    }

    private Vector3 Predict()
    {
        float g = -Physics.gravity.magnitude;

        Vector2 target = Random.insideUnitCircle * projShootRadius;
        Vector3 vec = new Vector3(target.x, transform.position.y, target.y) + transform.position;
        if(Random.Range(0f, 100f) < directPercentage)
            vec = player.position;
        Vector3 s = vec - shootTrans.position;
        float h = s.y + Random.Range(projHeight - 2f, projHeight + 5f);
        if(h < projHeight)
            h = projHeight;
        Vector3 disInXZ = new Vector3(s.x, 0f, s.z);
        Vector3 velY = Vector3.up * Mathf.Sqrt(-2 * g * h);
        Vector3 velXZ = disInXZ / (Mathf.Sqrt(-2 * h/g) + Mathf.Sqrt(2 *(s.y - h)/g));
        return velY + velXZ;
    }
}
