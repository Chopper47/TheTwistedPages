using UnityEngine;

public class IAction : MonoBehaviour
{
    /// <summary>
    /// Abstract class for calling Act from the DecisionTreeAction, usually overrited for a new action
    /// </summary>
    public virtual void Act()
    {
        //Debug.Log("null - " + gameObject.name);
        return;
    }
}
