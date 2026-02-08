using UnityEngine;

public class ConditionRandomChance : ICondition
{
    [Range(0f, 1f)]
    public float probability = 0.5f;

    public override bool Check()
    {
        return Random.value <= probability;
    }
}
