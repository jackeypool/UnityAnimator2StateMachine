using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing a condition on which state transitions occur.
/// This is an abstract class. You use it by deriving your own component from it 
/// and implementing IsSatisfied().
/// </summary>
public abstract class GsConditionInterface
{
	//Determines whether the condition is satisfied.
	public abstract bool IsSatisfied (GsStateMachine stateMachine, GsState ownerState, GsStateTransition transition);
}

[System.Serializable]
public class GsTransitionCondition : GsConditionInterface
{
    public bool exitTimeEnable;
    public float exitTime;
    public GsConditionChecker[] checkers;

    //if last frame reach exit time
    private bool cachedResult = false;
    public override bool IsSatisfied(GsStateMachine stateMachine, GsState ownerState, GsStateTransition transition)
    {
        string fromState = ownerState.name;
        string toState = transition.nextStateName;

        if (!transition.canTransitionToSelf && ownerState.name == transition.nextStateName)
        {
            return false;
        }
        //last frame is not reached and this frame reach exit time then exit time condition meet
        //or return false
        if (exitTimeEnable) 
        {
            bool nowResult = ownerState.runningTime > exitTime * ownerState.length - 0.1f;
            if (!cachedResult && nowResult)
            {
                cachedResult = nowResult;
            }
            else
            {
                cachedResult = nowResult;
                return false;
            }
        }

        //遍历checker， 任何一个不满足就 return false 跳出
        for (int i = 0; i < checkers.Length; i++)
        {
            GsConditionChecker checker = checkers[i];
            Parameter param = stateMachine.parameters[checker.paramName];
            bool boolParam = param.boolValue;
            int intParam = param.intValue;
            float floatParam = param.floatValue;
            switch (param.paramType)
            {
                case ParamType.ParamType_Bool:
                    if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_If && boolParam == false)
                    {
                        return false;
                    }
                    else if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_IfNot && boolParam == true)
                    {
                        return false;
                    }
                    break;
                case ParamType.ParamType_Trigger:
                    if (boolParam == false)
                    {
                        return false;
                    }
                    param.boolValue = false;
                    break;
                case ParamType.ParamType_Int:
                    if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_Equals && intParam != (int)checker.threshold)
                    {
                        return false;
                    }
                    else if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_NotEqual && intParam == (int)checker.threshold)
                    {
                        return false;
                    }
                    else if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_Greater && intParam <= (int)checker.threshold)
                    {
                        return false;
                    }
                    else if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_Less && intParam >= (int)checker.threshold)
                    {
                        return false;
                    }
                    break;
                case ParamType.ParamType_Float:
                    if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_Equals && !Mathf.Approximately(floatParam,checker.threshold))
                    {
                        return false;
                    }
                    else if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_NotEqual && Mathf.Approximately(floatParam, checker.threshold))
                    {
                        return false;
                    }
                    else if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_Greater && floatParam <= checker.threshold)
                    {
                        return false;
                    }
                    else if (checker.checkMode == GsConditionChecker.CheckMode.CheckMode_Less && floatParam >= checker.threshold)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }
}
    
