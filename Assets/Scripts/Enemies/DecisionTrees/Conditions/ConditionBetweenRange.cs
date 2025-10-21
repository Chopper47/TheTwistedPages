using UnityEditor.Timeline;
using UnityEngine;

public class ConditionWolfBetweenRange : ICondition
{
    [SerializeField] float minRange = 1.5f;
    [SerializeField] float maxRange = 3.5f;

    [SerializeField] ChaseAgent chase;

    public void Awake()
    {
        if (!chase)
        { 
            chase = transform.root.GetComponent<ChaseAgent>();
        }
    }

    public override bool Check()
    {
        return chase.DistanceFromTarget() < maxRange && chase.DistanceFromTarget() >= minRange;
    }
}
