using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlyer
{
    void StartFlight(AnimationCurve curve, ref float time);
    void DoFlight(float height);
    void EndFlight(ref float time);
}
