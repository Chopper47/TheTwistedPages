using UnityEngine;

public class ActionLookAt : IAction
{
    [SerializeField] ChaseAgent agent;
    public override void Act()
    {
        agent.LookAtTarget();
    }
}
