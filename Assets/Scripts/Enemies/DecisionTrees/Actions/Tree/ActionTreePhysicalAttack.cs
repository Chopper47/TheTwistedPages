using UnityEngine;

public class ActionTreePhysicalAttack : IAction
{
    private ChaseAgent chaseAgent;
    private Tree tree;

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
        if (!tree.IsSlamming())
        {
            chaseAgent.StopChase();
            tree.SetSlamming(true);
            Debug.Log("Tree does physical attack: " + gameObject.name);
        }
    }
}
