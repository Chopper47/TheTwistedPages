using UnityEngine;

public class ConditionTargetState : ICondition
{
    [SerializeField] ChaseAgent enemy;

    /// <summary>
    /// Method that call TargetExist method from chaseAgent
    /// </summary>
    /// <returns>Returns TargetExits value</returns>
    public override bool Check()
    {
        return enemy.TargetExist();
    }
}
