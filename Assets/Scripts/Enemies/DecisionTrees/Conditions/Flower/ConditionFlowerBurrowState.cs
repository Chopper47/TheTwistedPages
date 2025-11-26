using UnityEngine;

public class ConditionFlowerBurrowState : ICondition
{
    [SerializeField] Flower flower;

    public override bool Check()
    {
        return flower.IsBurrowed();
    }
}
