using UnityEngine;

public class ActionThrowAttackB : IAction
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
        Debug.Log("Lumberjack: Throw Attack B");
        if(!lumberjack.isAttacking)
        {
            lumberjack.DoThrowAttackB();
        }
    }
}
