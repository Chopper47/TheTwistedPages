using UnityEngine;

public class ActionLaunch : IAction
{
    public override void Act()
    {
        Debug.Log("Lauch");
        base.Act();
    }
}
