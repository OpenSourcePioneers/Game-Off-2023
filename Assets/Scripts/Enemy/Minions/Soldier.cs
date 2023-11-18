using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Enemy
{
    [SerializeField] private CollisionCheck head;
    [SerializeField] private Transform ppHole;
    [SerializeField] private float flightActivationRange;
    [SerializeField] private float heightOfFlight;

    Vector3 target;
    float curTime;
    float baseElevation;
    float defSpeed;

    void Start()
    {
        InitializeStates();
        foreach (AttackSOBase attack in instAttackBase)
        {
            attack.shootTrans = ppHole;
            attack.head = head;
        }
        curTime = 0f;
        defSpeed = speed;
    }

    void Update()
    {
        UpdateCall();
        
        if(vec.magnitude > flightActivationRange && canTransition)
        {
            canTransition = false;
            enemyRb.velocity = Predict();
            target = vec + transform.position;
        }else if((transform.position - target).magnitude < grid.nodeRadius && !canTransition)
        {
            canTransition = true;
        }
    }

    private Vector3 Predict()
    {
        //Initialize variables
        Vector3 s = vec;
        float g = -Physics.gravity.magnitude;
        float h = s.y + heightOfFlight;
        if(h < heightOfFlight)
            h = heightOfFlight;
        //Calculate
        Vector3 disInXZ = new Vector3(s.x, 0f, s.z);
        Vector3 velY = Vector3.up * Mathf.Sqrt(-2 * g * h);
        Vector3 velXZ = disInXZ / (Mathf.Sqrt(-2 * h/g) + Mathf.Sqrt(2 *(s.y - h)/g));
        return velY + velXZ;
    }
}
