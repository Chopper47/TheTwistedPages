using UnityEngine;

public class ActionPursueBoss : IAction
{
    [SerializeField] ChaseAgent agent;
    [SerializeField] LumberjackAnimations animations;

    public override void Act()
    {
        agent.Chase();
        animations.Moving();
    }
}
