using UnityEngine;

public class ConditionNearEnough : ICondition
{
    [SerializeField] ChaseAgent enemy;

    [Header("Chase Distances")]
    [SerializeField] float startChaseDistance = 4.0f;
    [SerializeField] float stopChaseDistance = 10.0f;

    private bool isChasing;

    public void Awake()
    {
        if (enemy == null)
        {
            enemy = transform.root.GetComponent<ChaseAgent>();
        }
    }

    /// <summary>
    /// Method for checking distance from the agent to its target
    /// </summary>
    /// <returns>True if agent distance from the target is less than float value</returns>
    public override bool Check()
    {
        float dist = enemy.DistanceFromTarget();

        if (!isChasing && dist < startChaseDistance)
        {
            isChasing = true;
        }

        if (isChasing && dist > stopChaseDistance)
        {
            isChasing = false;
        }

        return isChasing;
    }
}
