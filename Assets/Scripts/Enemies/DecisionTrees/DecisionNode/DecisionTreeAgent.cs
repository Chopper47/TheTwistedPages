using UnityEngine;

public class DecisionTreeAgent : MonoBehaviour
{
    [SerializeField] DecisionTreeNode node;

    IAction action;

    /// <summary>
    /// Update, if there's no node asigned ignores the rest, if there is sets the action to the Decide() method return value
    /// from variable node, if the action isnt null it calls the Act method
    /// </summary>
    void Update()
    {
        if(!node)
        {
            return;
        }

        action = node.Decide();

        if (action != null) 
        {
            action.Act();
        }
    }
}
