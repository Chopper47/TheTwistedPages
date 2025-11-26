using UnityEngine;

public class ConditionTimeBurrowed : ICondition
{
    [SerializeField] Flower flower;

    public override bool Check()
    {
        return flower.TimePassed();
    }
}
