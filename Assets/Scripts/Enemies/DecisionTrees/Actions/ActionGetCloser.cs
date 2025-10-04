using UnityEngine;

public class ActionGetCloser : IAction
{
    [SerializeField] ChaseAgent enemy;

    /// <summary>
    /// Act calls the Chase method from the ChaseAgent script
    /// </summary>
    public override void Act()
    {
        Debug.Log("chasing " + " - " + gameObject.name);
        enemy.Chase();
    }
}
