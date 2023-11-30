using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [SerializeField] private bool freeType = false;
    AttackSOBase attackBehaviour;
    public Enemy enemy;
    Collider mainCollider;
    Collider thisCollider;

    // Start is called before the first frame update
    void Start()
    {
        if(enemy == null)
            enemy = GetComponentInParent<Enemy>();
        mainCollider = enemy.GetComponent<Collider>();
        thisCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(attackBehaviour == null || other == mainCollider || (!freeType && enemy.WalkState))
            return;
        attackBehaviour.DoCollisionCheck(thisCollider, other);
    }

    public void AssignBehaviour(AttackSOBase behaviour)
    {
        attackBehaviour = behaviour;
    }
}
