using UnityEngine;

public class ConditionFlowerBurrowChance : ICondition
{
    [SerializeField]Flower flower;

    public override bool Check()
    {
        if (!flower.CurrentlyChecking())
        {
            flower.CallBurrow();
        }
        return flower.BurrowCheck();
    }
}
