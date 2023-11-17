using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Enemy
{
    [SerializeField] private CollisionCheck head;
    [SerializeField] private Transform ppHole;
    [SerializeField] private AnimationCurve moveFlighCurve;
    [SerializeField] private float flightActivationRange;
    [SerializeField] private float flightSpeed;

    bool flightEnded = true;
    bool flightStarted = true;
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
        
        if(vec.magnitude > flightActivationRange || flightStarted)
        {
            StartFlight(moveFlighCurve, ref curTime);
        }
        if(changedVec)
            EndFlight();
    }

    private void StartFlight(AnimationCurve curve, ref float time)
    {
        if(!flightStarted)
        {
            speed = flightSpeed;
            baseElevation = transform.position.y;
        }
        enemyRb.useGravity = false;
        time += Time.deltaTime;
        flightStarted = true;
        enemyRb.position = new Vector3(transform.position.x + enemyRb.velocity.x, baseElevation + 
            curve.Evaluate(time), transform.position.z + enemyRb.velocity.z);
    }

    private void EndFlight()
    {
        enemyRb.useGravity = true;
        flightStarted = false;
        flightEnded = true;
        curTime = 0f;
        speed = defSpeed;
    }
}
