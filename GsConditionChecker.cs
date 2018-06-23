using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GsConditionChecker
{

    public enum CheckMode
    {
        CheckMode_If = 1,
        CheckMode_IfNot = 2,
        CheckMode_Greater = 3,
        CheckMode_Less = 4,
        CheckMode_Equals = 6,
        CheckMode_NotEqual = 7,
    };
    public CheckMode checkMode;
    public string paramName;
    public float threshold;
}
