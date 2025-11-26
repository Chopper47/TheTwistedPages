using UnityEngine;

public class ConditionFlowerBurrowChance : ICondition
{
    [SerializeField]Flower flower;

    public override bool Check()
    {
        return flower.ChanceBurrow();
    }
}
