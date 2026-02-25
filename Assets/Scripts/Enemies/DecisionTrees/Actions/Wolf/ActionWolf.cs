using UnityEngine;

public class ActionWolf : IAction
{
    bool firstTime = true;
    [SerializeField] Wolf wolf;
    public override void Act()
    {
        if (firstTime)
        {
            firstTime = false;
            wolf.Howl();
        }
    }
}
