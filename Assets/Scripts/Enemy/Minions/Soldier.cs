using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Enemy, IIsAFlyer
{
    [SerializeField] private CollisionCheck head;
    [SerializeField] private Transform ppHole;
    [SerializeField] private AnimationCurve moveFlighCurve;
    [SerializeField] private float flightActivationRange;
    [SerializeField] private float heightOfFlight;
    [SerializeField] private float flightSpeed;

    Vector3 target;
    float curTime;
    float baseElevation;
    bool flightEnded = true;
    [HideInInspector] public bool flightStarted = false;

    void Start()
    {
        InitializeStates();
        foreach (AttackSOBase attack in instAttackBase)
        {
            attack.shootTrans = ppHole;
            attack.head = head;
        }
        curTime = 0f;
    }

    void Update()
    {
        UpdateCall();
        if(WalkState)
        {
            if(vec.magnitude > flightActivationRange || flightStarted)
                StartFlight(moveFlighCurve, ref curTime);
        }
        if((changedVec && flightStarted) || !canTransition)
            EndFlight(ref curTime);
    }

    void FixedUpdate()
    {
        DoFlight(heightOfFlight);
        FixedUpdateCall();
    }

    public void StartFlight(AnimationCurve curve, ref float time)
    {
        if(!flightStarted)
        {
            baseElevation = transform.position.y;
            enemyRb.useGravity = false;
            speed = flightSpeed;
        }

        flightStarted = true;
        target = transform.up * curve.Evaluate(time);
        time += Time.deltaTime;

    }

    public void DoFlight(float height, float randomness = 1f)
    {
        if(flightStarted)
        {
            if(transform.position.y < Random.Range(height, height + randomness))
            {
                enemyRb.MovePosition(transform.position + target);
            }
            else
            {
                enemyRb.MovePosition(transform.position - target);
            }
        }
    }

    public void EndFlight(ref float time)
    {
        enemyRb.useGravity = true;
        speed = defSpeed;
        flightStarted = false;
        flightEnded = true;
        time = 0f;
    }
}
