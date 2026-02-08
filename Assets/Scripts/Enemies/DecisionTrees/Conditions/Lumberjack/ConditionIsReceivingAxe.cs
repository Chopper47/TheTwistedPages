using UnityEngine;

public class ConditionIsReceivingAxe : ICondition
{
    [SerializeField] private Lumberjack lumberjack;

    private void Awake()
    {
        if (!lumberjack)
        {
            lumberjack = transform.root.GetComponent<Lumberjack>();
        }
    }

    public override bool Check()
    {
        bool result = lumberjack.axeReturning;
        Debug.Log("ConditionReceivingAxe: " + result);
        return result;
    }
}
