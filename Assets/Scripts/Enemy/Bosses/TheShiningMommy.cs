using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheShiningMommy : Beetle
{
    void Awake()
    {
        AssignComponentsForBosses();
    }
    
    void Start()
    {
        InitializeStatesForBosses();
    }
}
