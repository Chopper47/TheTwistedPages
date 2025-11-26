using UnityEngine;

public class ConditionFlowerInRange : ICondition
{
    [SerializeField] Flower flower;

    public override bool Check()
    {
        return flower.InShootRange();
    }
}
