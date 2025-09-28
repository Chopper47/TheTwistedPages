using UnityEngine;

public class ConditionTargetState : ICondition
{
    [SerializeField] ChaseAgent enemy;
    public override bool Check()
    {
        return enemy.TargetExist();
    }
}
