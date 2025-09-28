using UnityEngine;

public class DecisionTreeAction : DecisionTreeNode
{
    [SerializeField] IAction action;
    public override IAction Decide()
    {
        return action;
    }
}
