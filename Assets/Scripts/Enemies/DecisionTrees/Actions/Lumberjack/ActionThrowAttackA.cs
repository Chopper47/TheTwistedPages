using UnityEngine;

public class ActionThrowAttackA : IAction
{
    [SerializeField] private Lumberjack lumberjack;

    private void Awake()
    {
        if (!lumberjack)
        {
            lumberjack = transform.root.GetComponent<Lumberjack>();
        }
    }

    public override void Act()
    {
        Debug.Log("Lumberjack: Throw Attack A");
        if (!lumberjack.isAttacking)
        {
            lumberjack.DoThrowAttackA();
        }
    }
}
