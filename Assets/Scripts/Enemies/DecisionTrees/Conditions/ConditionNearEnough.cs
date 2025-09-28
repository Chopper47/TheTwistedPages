using UnityEngine;

public class ConditionNearEnough : ICondition
{
    [SerializeField] ChaseAgent enemy;
    [SerializeField] float distance = 4f;
    public override bool Check()
    {
        return enemy.DistanceFromTarget() < distance;
    }
}
