using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [SerializeField] private AttackSOBase attackBehaviour;
    Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        attackBehaviour.DoCollisionCheck(other);
    }
}
