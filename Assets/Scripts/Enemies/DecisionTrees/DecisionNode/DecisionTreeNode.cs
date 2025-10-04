using UnityEngine;

public class DecisionTreeNode : MonoBehaviour
{
    /// <summary>
    /// Abstract class for DecisionTree nodes, agent calls Decide method, different for each DecisionTreeNode
    /// </summary>
    /// <returns></returns>
    public virtual IAction Decide()
    {
        return null;
    }
}
