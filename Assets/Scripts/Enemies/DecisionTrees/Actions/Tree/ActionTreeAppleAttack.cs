using UnityEngine;

public class ActionTreeAppleAttack : IAction
{
    [SerializeField] private ChaseAgent chaseAgent;
    [SerializeField] private Tree tree;

    private void Awake()
    {
        if (!chaseAgent)
        {
            chaseAgent = transform.root.GetComponent<ChaseAgent>();
        }

        if (!tree)
        {
            tree = transform.root.GetComponent<Tree>();
        }
    }

    public override void Act()
    {
        if (!tree.IsThrowing() && tree.CanThrow())
        {
            chaseAgent.StopChase();
            tree.SetThrowing(true);
            //Debug.Log("Tree does apple attack: " + gameObject.name);
        }
    }
}
