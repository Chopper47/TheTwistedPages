using UnityEngine;

public class ActionGetCloser : IAction
{
    [SerializeField] ChaseAgent enemy;
    public override void Act()
    {
        Debug.Log("chasing " + " - " + gameObject.name);
        enemy.Chase();
    }
}
