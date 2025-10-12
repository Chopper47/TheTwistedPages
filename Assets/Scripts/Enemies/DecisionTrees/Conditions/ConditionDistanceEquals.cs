using UnityEngine;

public class ConditionDistanceEquals : ICondition
{
    [SerializeField] ChaseAgent enemy;
    [SerializeField] float distance = 4f;
    [SerializeField] float variation = 0.1f;

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
        return enemy.DistanceFromTarget() > distance - variation && enemy.DistanceFromTarget() < distance + variation;
    }
}
