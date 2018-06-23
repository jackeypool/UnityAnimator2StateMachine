using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
public class Animator2Asset
{
    [MenuItem("Assets/CustomTools/Animator2Asset")]
    static void AnimatorConvetToAsset()
    {
        var animatorController = Selection.activeObject as AnimatorController;
        AnimatorControllerLayer layer = animatorController.layers[0];
        AnimatorStateMachine sm = layer.stateMachine;
        ChildAnimatorState[]  states = sm.states; 
        float exitTime = states[1].state.transitions[0].exitTime;
        //states[1].state.transitions[0].destinationState
        //animatorController.parameters[0].type
        //AnimationClip clip = states[1].state.motion as AnimationClip;
        //Debug.Log(clip.length);
        //Debug.Log("contert done, states " + states[1].state.name + " exit time is " + states[1].state.transitions[0].exitTime);


        GsStateMachineData data = ScriptableObject.CreateInstance<GsStateMachineData>();
        //填充State
        data.States = new StateEntity[states.Length];
        for (int i = 0; i < states.Length; i++)
        {
            AnimationClip clip = states[i].state.motion as AnimationClip;
            data.States[i] = new StateEntity();
            
            data.States[i].State = new GsState();
            data.States[i].State.name = clip.name;
            data.States[i].State.length = clip.length;
            data.States[i].State.loop = clip.isLooping;

            data.States[i].State.enabled = false;
            data.States[i].State.runningTime = 0f;

            AnimatorStateTransition[] transitions = states[i].state.transitions;
            data.States[i].Transitions = new GsStateTransition[transitions.Length];
            for (int j = 0; j < transitions.Length; j++)
            {
                data.States[i].Transitions[j] = new GsStateTransition();
                data.States[i].Transitions[j].duration = transitions[j].duration;
                data.States[i].Transitions[j].canTransitionToSelf = transitions[j].canTransitionToSelf;
                //data.States[i].Transitions[j].Priority = j;
                data.States[i].Transitions[j].nextStateName = transitions[j].destinationState.motion.name;

                data.States[i].Transitions[j].Cond = new GsTransitionCondition();
                data.States[i].Transitions[j].Cond.exitTime = transitions[j].exitTime;
                data.States[i].Transitions[j].Cond.exitTimeEnable = transitions[j].hasExitTime;
                
                AnimatorCondition[] conditions = transitions[j].conditions;
                data.States[i].Transitions[j].Cond.checkers = new GsConditionChecker[conditions.Length];
                for (int k = 0; k < conditions.Length; k++)
                {
                    data.States[i].Transitions[j].Cond.checkers[k] = new GsConditionChecker();
                    data.States[i].Transitions[j].Cond.checkers[k].paramName = conditions[k].parameter;
                    data.States[i].Transitions[j].Cond.checkers[k].threshold = conditions[k].threshold;
                    int mode = (int)conditions[k].mode;
                    data.States[i].Transitions[j].Cond.checkers[k].checkMode = (GsConditionChecker.CheckMode)mode;
                }
            }
        }

        data.DefaultStateName = sm.entryTransitions[0].destinationState.motion.name;

        //填充parameters
        AnimatorControllerParameter[] parameters = animatorController.parameters;
        data.Parameters = new Parameter[parameters.Length];
        for (int h = 0; h < parameters.Length; h++)
        {
            int type = (int)parameters[h].type;
            data.Parameters[h] = new Parameter((ParamType)type);
            data.Parameters[h].name = parameters[h].name;
            data.Parameters[h].boolValue = parameters[h].defaultBool;
            data.Parameters[h].intValue = parameters[h].defaultInt;
            data.Parameters[h].floatValue = parameters[h].defaultFloat;
        }

        //Fill any state transition
        AnimatorStateTransition[] anyStateTransitions = sm.anyStateTransitions;
        data.AnyStateTransitions = new GsStateTransition[anyStateTransitions.Length];
        for (int j = 0; j < anyStateTransitions.Length; j++)
        {
            data.AnyStateTransitions[j] = new GsStateTransition();
            data.AnyStateTransitions[j].duration = anyStateTransitions[j].duration;
            data.AnyStateTransitions[j].canTransitionToSelf = anyStateTransitions[j].canTransitionToSelf;
            //data.AnyStateTransitions[j].Priority = j;
            data.AnyStateTransitions[j].nextStateName = anyStateTransitions[j].destinationState.motion.name;

            data.AnyStateTransitions[j].Cond = new GsTransitionCondition();
            data.AnyStateTransitions[j].Cond.exitTime = anyStateTransitions[j].exitTime;
            data.AnyStateTransitions[j].Cond.exitTimeEnable = anyStateTransitions[j].hasExitTime;

            AnimatorCondition[] conditions = anyStateTransitions[j].conditions;
            data.AnyStateTransitions[j].Cond.checkers = new GsConditionChecker[conditions.Length];
            for (int k = 0; k < conditions.Length; k++)
            {
                data.AnyStateTransitions[j].Cond.checkers[k] = new GsConditionChecker();
                data.AnyStateTransitions[j].Cond.checkers[k].paramName = conditions[k].parameter;
                data.AnyStateTransitions[j].Cond.checkers[k].threshold = conditions[k].threshold;
                int mode = (int)conditions[k].mode;
                data.AnyStateTransitions[j].Cond.checkers[k].checkMode = (GsConditionChecker.CheckMode)mode;
            }
        }
        string path = AssetDatabase.GetAssetPath(animatorController).Replace(".controller", ".asset");
        AssetDatabase.CreateAsset(data, path);
    }
}

