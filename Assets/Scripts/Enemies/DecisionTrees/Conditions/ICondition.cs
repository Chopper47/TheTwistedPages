using UnityEngine;

public class ICondition : MonoBehaviour
{
    /// <summary>
    /// Abstract class for conditions, Method check usually overriten when creating new conditions
    /// </summary>
    /// <returns></returns>
    public virtual bool Check()
    {
        return false;
    }
}
