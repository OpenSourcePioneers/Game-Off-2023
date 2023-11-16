using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : Enemy
{
    
    [SerializeField] private CollisionCheck head;
    //[SerializeField] public CollisionCheck head;
    private void Start() 
    {
        InitializeStates();
        foreach (AttackSOBase attack in instAttackBase)
        {
            attack.head = head;
        }
    }
}
