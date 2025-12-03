using UnityEngine;

public class ConditionIsSlamming : ICondition
{
    [SerializeField] Tree tree;

    public override bool Check()
    {
        return tree.IsSlamming();
    }
}
