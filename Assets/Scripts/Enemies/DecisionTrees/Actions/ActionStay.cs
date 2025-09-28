using UnityEngine;

public class ActionStay : IAction
{
    [SerializeField]ChaseAgent enemy;
    public override void Act()
    {
        enemy.Stay();
    }
}
