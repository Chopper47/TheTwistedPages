using System;
using UnityEngine;

public class BinaryDecision : DecisionTreeNode
{
    [SerializeField] ICondition condition;

    [SerializeField] DecisionTreeNode falseNode, trueNode;
    
    /// <summary>
    /// Checks first if condition exists, if it does based on the value from the Check() method of the condition decides 
    /// which node to call.
    /// </summary>
    /// <returns></returns>
    public override IAction Decide()
    {
        if(condition == null)
        {
            Debug.LogError("No condition added!");
            return null;
        }
        else
        {
            if(condition.Check())
            {
                if(trueNode!=null)
                {
                    return trueNode.Decide();
                }
                else
                {
                    Debug.LogError("No true node assigned!");
                    return null;
                }
            }
            else
            {
                if (falseNode != null)
                {
                    return falseNode.Decide();
                }
                else
                {
                    Debug.LogError("No false node assigned!");
                    return null;
                }
            }
        }
    }
}
