using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    AttackSOBase attackBehaviour;
    public Enemy enemy;
    Collider mainCollider;
    // Start is called before the first frame update
    void Start()
    {
        if(enemy == null)
            enemy = GetComponentInParent<Enemy>();
        mainCollider = enemy.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(attackBehaviour == null || other == mainCollider)
            return;
        attackBehaviour.DoCollisionCheck(other);
    }

    public void AssignBehaviour(AttackSOBase behaviour)
    {
        attackBehaviour = behaviour;
    }
}
