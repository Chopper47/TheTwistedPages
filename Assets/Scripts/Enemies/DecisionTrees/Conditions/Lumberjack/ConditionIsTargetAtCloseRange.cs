using UnityEngine;

public class ConditionIsTargetAtCloseRange : ICondition
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
        bool result = lumberjack.TargetInCloseRange();
        Debug.Log("ConditionTargetCloseRange: " + result);
        return result;
    }
}
