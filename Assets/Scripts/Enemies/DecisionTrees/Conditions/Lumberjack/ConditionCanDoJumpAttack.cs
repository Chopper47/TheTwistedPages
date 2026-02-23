using UnityEngine;

public class ConditionCanDoJumpAttack : ICondition
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
        bool result = lumberjack.CanDoJumpAttack();
        Debug.Log("ConditionCanDoJumpAttack: " + result);
        return result;
    }
}
