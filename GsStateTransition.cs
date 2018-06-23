using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//state transition info
[System.Serializable]
public class GsStateTransition
{
    [Tooltip("Condition on which the transition will happen.")]
    public GsTransitionCondition Cond;

    [Tooltip("When multiple conditions are satisfied, transition with the largest priority will take place.")]
    public int Priority;

    [Tooltip("Time in second the transition will last.")]
    public float duration;

    [Tooltip("Can the next and current state be the same?")]
    public bool canTransitionToSelf;

    [Tooltip("Destination state")]
    public string nextStateName;
}