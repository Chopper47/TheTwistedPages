using UnityEngine;

public class DecisionTreeAgent : MonoBehaviour
{
    [SerializeField] DecisionTreeNode node;

    IAction action;

    bool decide = true;
    /// <summary>
    /// Update, if there's no node asigned ignores the rest, if there is sets the action to the Decide() method return value
    /// from variable node, if the action isnt null it calls the Act method
    /// </summary>
    void Update()
    {
        if(decide)
        {
            if(!node)
            {
                return;
            }

            action = node.Decide();

            if (action != null) 
            {
                action.Act();
                Debug.Log(action.name +" - "+ action.transform.parent.name);
            }
        }
    }

    public void SetDecide(bool newDecide)
    {
        decide = newDecide;
    }
}
