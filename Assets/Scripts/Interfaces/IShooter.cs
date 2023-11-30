using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShooter
{
    int maxShots{get; set;}
    int shotsFired{get; set;}
    float delay{get; set;}
    float timeSinceLastShot{get; set;}
}

