using UnityEngine;

public class ConditionCanLaunch : ICondition
{
    [SerializeField] Wolf wolf;

    /// <summary>
    /// Returns the value of the bool variable CanLaunch from wolf
    /// </summary>
    /// <returns></returns>
    public override bool Check()
    {
        return wolf.CanLaunch();
    }
}
