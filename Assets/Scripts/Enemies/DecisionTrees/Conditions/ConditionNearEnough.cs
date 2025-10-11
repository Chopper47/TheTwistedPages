using UnityEngine;

public class ConditionNearEnough : ICondition
{
    [SerializeField] ChaseAgent enemy;
    [SerializeField] float distance = 4f;

    public void Start()
    {
        enemy = transform.root.GetComponent<ChaseAgent>();
    }
    /// <summary>
    /// Method for checking distance from the agent to its target
    /// </summary>
    /// <returns>True if agent distance from the target is less than float value</returns>
    public override bool Check()
    {
        return enemy.DistanceFromTarget() < distance;
    }
}
