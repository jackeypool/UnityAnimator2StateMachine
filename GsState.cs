using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing an FSM state.
/// This is an abstract class. You use it by deriving your own component
/// from it. You may implement methods OnStateEnter() and OnStateExit().
/// By default, these methods have an empty body.
/// </summary>
/// 
[System.Serializable]
public class GsState
{
    public string name;
    public string clip;
    public float length;
    public bool loop;

    [System.NonSerialized]
    public bool enabled;
    [System.NonSerialized]
    public float runningTime;

    /// <summary>
    /// Called just after the state is entered.
    /// </summary>
    public void OnStateEnter ()
    {
        runningTime = 0f;
        //Debug.Log("state enter : " + name);
	}

	/// <summary>
	/// Called just before the state is left.
	/// </summary>
	public void OnStateExit ()
    {
    }


    /// <summary>
    /// Called just before the state is running.
    /// </summary>
    public void OnStateUpdate(float deltaTime)
    {
        runningTime += deltaTime;
        if (runningTime > length && loop == false)
        {
            runningTime = 0f;
        }
    }
}
