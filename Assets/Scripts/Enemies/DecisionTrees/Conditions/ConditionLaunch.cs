using UnityEditor.Timeline;
using UnityEngine;

public class ConditionLaunch : ICondition
{
    [SerializeField] WolfIA wolf;

    public override bool Check()
    {
        return wolf.IsLaunching();
    }
}
