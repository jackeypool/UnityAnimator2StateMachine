using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//有限状态机的轻量级实现, 可以导入Animator的数据， 实现逻辑模拟Animator
public class GsStateMachine : MonoBehaviour
{
    public delegate void OnStateChanged(GsState oldState, GsState newState, GsStateTransition transition);
    public OnStateChanged onStateChanged = null;

    public GsStateMachineData data;
    private GsStateMachineData instanceData;
   
    private GsStateTransition[] AnyStateTransitions;
    private StateEntity[] States;
    private string DefaultStateName;

    [HideInInspector]
    [System.NonSerialized]
    public Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();

    private StateEntity current;

    private Dictionary<string, StateEntity> stateDic = new Dictionary<string, StateEntity>();

    private void LoadData()
    {
        instanceData = GameObject.Instantiate(data);
        States = instanceData.States;
        DefaultStateName = instanceData.DefaultStateName;
        AnyStateTransitions = instanceData.AnyStateTransitions;

        for (int i = 0; i < instanceData.Parameters.Length; i++)
        {
            parameters[instanceData.Parameters[i].name] = instanceData.Parameters[i];
        }
    }
    // Use this for initialization
    void Start()
    {
        LoadData();

        if (States.Length > 0)
        {
            for (int i = 0; i < States.Length; ++i)
            {
                if (States[i].State != null)
                {
                    States[i].State.enabled = false;
                    stateDic[States[i].State.name] = States[i];
                }
            }
        }
        enterState(stateDic[DefaultStateName]);
    }

    void LateUpdate()
    {
        if (current == null)
            return;

        GsStateTransition chosenTransition = null;

        for (int i = 0; i < AnyStateTransitions.Length; ++i)
        {
            GsStateTransition tr = AnyStateTransitions[i];
            
            if (tr != null
                && (chosenTransition == null || tr.Priority > chosenTransition.Priority)
                && (tr.Cond != null && tr.Cond.IsSatisfied(this, current.State, tr))
                )
                chosenTransition = tr;
        }

        //any state transition priority > current state
        if (chosenTransition == null && current != null && current.Transitions != null)
        {
            for (int i = 0; i < current.Transitions.Length; ++i)
            {
                GsStateTransition tr = current.Transitions[i];
                if (tr != null &&
                    (chosenTransition == null || tr.Priority > chosenTransition.Priority) &&
                    (tr.Cond != null && (tr.Cond.IsSatisfied(this, current.State, tr)))
                    )
                    chosenTransition = tr;
            }
        }

        if (chosenTransition != null)
        {
            leaveCurrentState();

            StateEntity nextStateEntity = stateDic[chosenTransition.nextStateName];
            if (onStateChanged != null)
            {
                onStateChanged(current.State, nextStateEntity.State, chosenTransition);
            }

            enterState(nextStateEntity);
        }
        else
        {
            current.State.OnStateUpdate(Time.deltaTime);
        }
    }

    private void leaveCurrentState()
    {
        if (current != null && current.State != null)
        {
            current.State.OnStateExit();
            current.State.enabled = false;
        }
    }

    private void enterState(StateEntity newState)
    {
        if (newState != null && newState.State != null)
        {
            newState.State.enabled = true;
            newState.State.OnStateEnter();
            current = newState;
        }
    }

    public StateEntity GetCurrentState()
    {
        return current;
    }

    public void SetTrigger(string paramName)
    {
        parameters[paramName].boolValue = true;
        parameters[paramName].paramType = ParamType.ParamType_Trigger;
    }
    public void SetInteger(string paramName, int value)
    {
        parameters[paramName].intValue = value;
        parameters[paramName].paramType = ParamType.ParamType_Int;
    }
    public void SetFloat(string paramName, float value)
    {
        parameters[paramName].floatValue = value;
        parameters[paramName].paramType = ParamType.ParamType_Float;
    }
    public void SetBool(string paramName, bool value)
    {
        parameters[paramName].boolValue = value;
        parameters[paramName].paramType = ParamType.ParamType_Bool;
    }
}
