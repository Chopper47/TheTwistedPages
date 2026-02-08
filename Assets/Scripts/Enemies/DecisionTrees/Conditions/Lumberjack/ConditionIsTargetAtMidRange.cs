using UnityEngine;

public class ConditionIsTargetAtMidRange : ICondition
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
        bool result = lumberjack.TargetInMidRange();
        Debug.Log("ConditionTargetMidRange: " + result);
        return result;
    }
}
