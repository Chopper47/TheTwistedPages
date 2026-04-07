using UnityEngine;

public class ConditionHasHowled : ICondition
{
    [SerializeField] Wolf wolf;
    public override bool Check()
    {
        return wolf.HasHowled();
    }
}
