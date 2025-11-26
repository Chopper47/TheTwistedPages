using UnityEngine;

public class ConditionFlowerTargetClose : ICondition
{
    [SerializeField] Flower flower;
    public override bool Check()
    {
        return true;
    }
}
