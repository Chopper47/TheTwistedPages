using UnityEngine;

public class ConditionCanLaunch : ICondition
{
    [SerializeField] Wolf wolf;

    private void Awake()
    {
        if (!wolf)
        {
            wolf = transform.root.GetComponent<Wolf>();
        }
    }

    /// <summary>
    /// Returns the value of the bool variable CanLaunch from wolf
    /// </summary>
    /// <returns></returns>
    public override bool Check()
    {
        return wolf.CanLaunch();
    }
}
