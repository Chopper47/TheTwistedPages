using UnityEngine;

public class ConditionIsAxeOnGround : ICondition
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
        bool result = lumberjack.axeOnGround;
        Debug.Log("ConditionAxeOnGround: " + result);
        return result;
    }
}
