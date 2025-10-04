using UnityEngine;

public class ActionStay : IAction
{
    [SerializeField]ChaseAgent enemy;

    /// <summary>
    /// Act calls the Stay method from the ChaseAgent script
    /// </summary>
    public override void Act()
    {
        Debug.Log("stay " + " - " + gameObject.name);
        enemy.Stay();
    }
}
