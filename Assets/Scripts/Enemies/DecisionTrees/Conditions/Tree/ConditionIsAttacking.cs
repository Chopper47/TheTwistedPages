using UnityEngine;

public class ConditionIsAttacking : ICondition
{
    [SerializeField] Tree tree;

    public override bool Check()
    {
        return tree.IsSlamming() || tree.IsThrowing();
    }
}
