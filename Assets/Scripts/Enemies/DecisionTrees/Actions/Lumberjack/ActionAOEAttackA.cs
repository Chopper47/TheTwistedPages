using UnityEngine;

public class ActionAOEAttackA : IAction
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
        Debug.Log("Lumberjack: AOE Attack A");
        lumberjack.DoAOEAttackA();
    }
}
