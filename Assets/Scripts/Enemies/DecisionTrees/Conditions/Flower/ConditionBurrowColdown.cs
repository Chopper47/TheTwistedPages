using UnityEngine;

public class ConditionBurrowColdown : ICondition
{
    [SerializeField] Flower flower;
    public override bool Check()
    {
        return flower.IsBurrowInCooldown();
    }
}
