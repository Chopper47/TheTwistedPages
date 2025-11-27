using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ConditionFlowerComeOutChance : ICondition
{
    [SerializeField] Flower flower;

    public override bool Check()
    {
        if(!flower.CurrentlyChecking())
        {
            flower.CallComeout();
        }
        return flower.ComeoutCheck();
    }
}
