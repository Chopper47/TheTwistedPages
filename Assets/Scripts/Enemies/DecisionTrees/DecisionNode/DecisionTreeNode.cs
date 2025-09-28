using UnityEngine;

public class DecisionTreeNode : MonoBehaviour
{
    public virtual IAction Decide()
    {
        return null;
    }
}
