using System.Collections.Generic;
using UnityEngine;

public class MultipleAction : IAction
{
    [SerializeField] List<IAction> ActionsToDo = new List<IAction>();

    public override void Act()
    {
        foreach (IAction action in ActionsToDo)
        {
            action.Act();
        }
    }
}
