using UnityEngine;

public class ActionBurrowCooldown : IAction
{
    [SerializeField]Flower flower;

    public override void Act()
    {
        Debug.LogWarning("Cooldown called");
        flower.StartBurrowCooldown();
    }
}
