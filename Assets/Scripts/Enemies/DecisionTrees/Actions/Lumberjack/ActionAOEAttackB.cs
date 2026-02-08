using UnityEngine;

public class ActionAOEAttackB : IAction
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
        Debug.Log("Lumberjack: AOE Attack B");
        lumberjack.DoAOEAttackB();
    }
}
