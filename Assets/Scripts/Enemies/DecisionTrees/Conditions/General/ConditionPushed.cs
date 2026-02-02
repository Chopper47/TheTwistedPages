using UnityEngine;

public class ConditionPushed : ICondition
{
    [SerializeField] EnemyState state;

    public override bool Check()
    {
        return state.IsBeingPushed();
    }
}
