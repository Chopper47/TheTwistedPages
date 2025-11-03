using UnityEngine;

public class ConditionWolfConsecutiveStepback : ICondition
{
    [SerializeField] Wolf wolf;

    private void Awake()
    {
        if(!wolf)
        {
            wolf = transform.root.GetComponent<Wolf>();
        }
    }

    public override bool Check()
    {
        return wolf.ConsectiveLimit();
    }
}
