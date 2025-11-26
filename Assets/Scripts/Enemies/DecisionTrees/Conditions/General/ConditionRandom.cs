using UnityEngine;

public class ConditionRandom : ICondition
{
    [SerializeField] [Range(0,100)] int percentage = 50;
    public override bool Check()
    {
        int i = Random.Range(1,101);

        if(i <= percentage)
        {
            return true;
        }

        return false;
    }
}
