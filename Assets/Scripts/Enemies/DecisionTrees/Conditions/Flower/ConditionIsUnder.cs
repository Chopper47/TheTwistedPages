using UnityEngine;

public class ConditionIsUnder : ICondition
{
    [SerializeField] Flower flower;

    public override bool Check()
    {
        return flower.IsUnderPlayer();
    }
}
