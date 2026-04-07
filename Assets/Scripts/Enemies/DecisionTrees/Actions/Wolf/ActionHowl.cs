using UnityEngine;

public class ActionHowl : IAction
{
    [SerializeField] Wolf wolf;
    public override void Act()
    {
        wolf.Howl();
    }
}
