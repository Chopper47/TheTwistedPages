using UnityEngine;

public class ActionJumpAttack : IAction
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
        Debug.Log("Lumberjack: Jump Attack");
        lumberjack.DoJumpAttack();
    }
}
