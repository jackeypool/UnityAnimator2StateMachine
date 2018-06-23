using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class StateEntity
{
    [Tooltip("State component itself.")]
    public GsState State;

    [Tooltip("List of transition rules leading away from this state.")]
    public GsStateTransition[] Transitions;
}

public enum ParamType
{
    ParamType_Float = 1,
    ParamType_Int = 3,
    ParamType_Bool = 4,
    ParamType_Trigger = 9,
};

[System.Serializable]
public class Parameter
{
    public Parameter(ParamType paramType)
    {
        this.paramType = paramType;
    }
    public string name;
    public ParamType paramType;
    public bool boolValue;
    public float floatValue;
    public int intValue;
}

public class GsStateMachineData : ScriptableObject
{
    public StateEntity[] States;
    public string DefaultStateName;
    public GsStateTransition[] AnyStateTransitions;
    //public Dictionary<string, Parameter> Parameters;
    public Parameter[] Parameters;
}
