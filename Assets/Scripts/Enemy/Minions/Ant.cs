using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : Enemy
{
    [SerializeField] private CollisionCheck head;
    [SerializeField] private Transform ppHole;
    
    void Start()
    {
        InitializeStates();
        foreach (AttackSOBase attack in instAttackBase)
        {
            attack.shootTrans = ppHole;
            attack.head = head;
        }
    }
}
