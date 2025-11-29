using Unity.VisualScripting;
using UnityEngine;

public class ActionChaseDelay : IAction
{
    [SerializeField]Flower flower;

    public override void Act()
    {
        flower.StartChaseDelay();
    }
}
