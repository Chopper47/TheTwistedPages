using System;
using UnityEngine;

public class BinaryDecision : DecisionTreeNode
{
    [SerializeField] ICondition condition;

    [SerializeField] DecisionTreeNode falseNode, trueNode;
    
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
