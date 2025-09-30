using UnityEditor.Timeline;
using UnityEngine;

public class ConditionLaunch : ICondition
{
    [SerializeField] WolfAI wolf;

    public override bool Check()
    {
        return wolf.IsLaunching();
    }
}
