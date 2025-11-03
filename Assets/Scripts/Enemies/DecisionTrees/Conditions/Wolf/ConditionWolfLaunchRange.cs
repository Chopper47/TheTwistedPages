using Unity.VisualScripting;
using UnityEngine;

public class ConditionWolfLaunchRange : ICondition
{
    [SerializeField] Wolf wolf;
    [SerializeField] ChaseAgent enemy;
    [SerializeField] float variation = 0.1f;
    private void Awake()
    {
        if (wolf == null) { wolf = transform.root.GetComponent<Wolf>(); }
        if (enemy == null) { enemy = transform.root.GetComponent<ChaseAgent>(); }
    }

    /// <summary>
    /// Method for checking distance from the agent to its target
    /// </summary>
    /// <returns>True if agent distance from the target is less than float value</returns>
    public override bool Check()
    {
        return enemy.DistanceFromTarget() < wolf.LaunchRange() + variation && enemy.DistanceFromTarget() >= wolf.LaunchRange() - variation;
    }
}
