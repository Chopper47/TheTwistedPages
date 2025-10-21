using UnityEngine;

public class ConditionWolfRange : ICondition
{
    [SerializeField] ChaseAgent chase;
    [SerializeField] Wolf wolf;

    public void Awake()
    {
        if (!chase)
        {
            chase = transform.root.GetComponent<ChaseAgent>();
        }

        if (!wolf)
        {
            wolf = transform.root.GetComponent<Wolf>();
        }
    }

    public override bool Check()
    {
        return chase.DistanceFromTarget() < wolf.MaxDistanceStepback() && chase.DistanceFromTarget() >= wolf.MinDistanceStepback();
    }
}
