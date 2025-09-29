using UnityEngine;

public class ConditionCanLaunch : ICondition
{
    [SerializeField] WolfIA wolf;
    public override bool Check()
    {
        return wolf.CanLaunch();
    }
}
