using UnityEditor.Timeline;
using UnityEngine;

public class ConditionLaunch : ICondition
{
    [SerializeField] Wolf wolf;

    public override bool Check()
    {
        return wolf.IsLaunching();
    }
}
