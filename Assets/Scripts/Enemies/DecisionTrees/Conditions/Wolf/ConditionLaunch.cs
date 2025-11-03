using UnityEngine;

public class ConditionLaunch : ICondition
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
    /// Method that calls the IsLaunching method from Wolf class
    /// </summary>
    /// <returns>IsLaunching return value</returns>
    public override bool Check()
    {
        return wolf.IsLaunching();
    }
}
