using UnityEngine;

public class ActionPursueBoss : IAction
{
    [SerializeField] ChaseAgent agent;

    public override void Act()
    {
        agent.BossChase();
    }
}
