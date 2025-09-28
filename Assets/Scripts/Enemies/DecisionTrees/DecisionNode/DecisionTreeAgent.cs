using UnityEngine;

public class DecisionTreeAgent : MonoBehaviour
{
    [SerializeField] DecisionTreeNode node;
    
    // Update is called once per frame
    void Update()
    {
        if(!node)
        {
            return;
        }

        IAction action = node.Decide();

        if (action != null) 
        {
            action.Act();
        }
    }
}
