using UnityEngine;

public class ActionSwipeAttack : IAction
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
        Debug.Log("Lumberjack: Swipe Attack");
        lumberjack.DoSwipeAttack();
    }
}
