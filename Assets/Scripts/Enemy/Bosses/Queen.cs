using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Enemy
{
    [SerializeField] private CollisionCheck head;
    [SerializeField] private Transform ppHole;
    [SerializeField] public TwoD_Grid gridForMinions;
    [Header("Phase-2")]
    [SerializeField] private List<AttackSOBase> attackSOBase2;
    [SerializeField] private CombatSOBase combatSOBase2;
    [SerializeField] private float speed2;
    
    bool in2ndPhase = false;

    void Awake()
    {
        AssignComponentsForBosses();
    }
    void Start()
    {
        InitializeStatesForBosses();
        foreach (AttackSOBase attack in instAttackBase)
        {
            attack.shootTrans = ppHole;
            attack.head = head;
        }
    }

    void Update()
    {
        UpdateCall();
        if(curHealth < maxHealth/2 && !in2ndPhase)
            Start2ndPhase();
    }

    private void Start2ndPhase()
    {
        //Instantiate states
        instAttackBase.Clear();
        foreach (AttackSOBase attack in attackSOBase2)
        {
            instAttackBase.Add(Instantiate(attack));
        }
        instCombatBase = Instantiate(combatSOBase2);

        //InitializeStates
        foreach (AttackSOBase instAttack in instAttackBase)
        {
            instAttack.Initialize(this, gameObject);
        }
        instCombatBase.Initialize(this, gameObject, true);
        speed = speed2;


        in2ndPhase = true;
    }
}
