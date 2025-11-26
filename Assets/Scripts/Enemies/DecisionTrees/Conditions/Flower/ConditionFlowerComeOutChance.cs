using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ConditionFlowerComeOutChance : ICondition
{
    [SerializeField] Flower flower;

    public override bool Check()
    {
        return flower.ChanceComeOut();
    }
}
