using UnityEngine;

public class ActionStay : IAction
{
    [SerializeField]ChaseAgent enemy;

    private void Awake()
    {
        if(!enemy)
        {
            enemy = transform.root.GetComponent<ChaseAgent>();
        }
    }

    /// <summary>
    /// Act calls the Stay method from the ChaseAgent script
    /// </summary>
    public override void Act()
    {
        Debug.Log("stay " + " - " + gameObject.name);
        enemy.Stay();
    }
}
