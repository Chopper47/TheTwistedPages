using UnityEngine;

public class ConditionIsStunnedOrAttacking : ICondition
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
        return lumberjack.isStunned || lumberjack.isAttacking;
    }
}
