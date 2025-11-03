using UnityEngine;

public class ConditionTargetState : ICondition
{
    [SerializeField] ChaseAgent enemy;

    private void Awake()
    {
        if (!enemy)
        {
            enemy = transform.root.GetComponent<ChaseAgent>();
        }
    }

    /// <summary>
    /// Method that call TargetExist method from chaseAgent
    /// </summary>
    /// <returns>Returns TargetExits value</returns>
    public override bool Check()
    {
        return enemy.TargetExist();
    }
}
