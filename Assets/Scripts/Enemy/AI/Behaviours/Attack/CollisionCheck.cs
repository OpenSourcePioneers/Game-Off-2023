using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    AttackSOBase attackBehaviour;
    Enemy enemy;
    Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(attackBehaviour == null)
            return;
        attackBehaviour.DoCollisionCheck(other);
    }

    public void AssignBehaviour(AttackSOBase behaviour)
    {
        attackBehaviour = behaviour;
    }
}
