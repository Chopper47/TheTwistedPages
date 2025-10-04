using UnityEngine;

public class DecisionTreeAction : DecisionTreeNode
{
    [SerializeField] IAction action;

    /// <summary>
    /// Returns the action so the agent can call its Act method
    /// </summary>
    /// <returns></returns>
    public override IAction Decide()
    {
        return action;
    }
}
