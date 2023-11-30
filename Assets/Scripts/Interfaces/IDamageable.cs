using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(float amount, Vector3 hitLoc, bool customConcussion = false);
    void Concussion(float amount);
    
    float maxHealth{get; set;}
    float curHealth{get; set;}
}
