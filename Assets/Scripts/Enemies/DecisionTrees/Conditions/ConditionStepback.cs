using UnityEngine;

public class ConditionStepback : ICondition
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
    /// Returns the value of the bool variable stepback from wolf
    /// </summary>
    /// <returns></returns>
    public override bool Check()
    {
        return wolf.IsSteppingBack();
    }
}
