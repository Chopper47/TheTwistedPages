using UnityEngine;

public class ConditionHasAxe : ICondition
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
        bool result = lumberjack.hasAxe;
        Debug.Log("ConditionHasAxe: " + result);
        return result;
    }
}
