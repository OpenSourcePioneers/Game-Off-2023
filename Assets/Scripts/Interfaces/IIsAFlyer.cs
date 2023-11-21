using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIsAFlyer
{
    void StartFlight(AnimationCurve curve, ref float time);
    void DoFlight(float height, float randomness = 1f);
    void EndFlight(ref float time);
}
