using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultipleAction : IAction
{
    [SerializeField] List<IAction> StartingActions = new List<IAction>();
    [SerializeField] List<IAction> ActionsToDo = new List<IAction>();
    [SerializeField] List<IAction> EndingActions = new List<IAction>(); 

    public override void Act()
    {
        foreach(IAction action in StartingActions)
        {
            action.Act();
        }

        foreach (IAction action in ActionsToDo)
        {
            action.Act();
        }

        foreach(IAction action in EndingActions)
        {
            action.Act();
        }
    }
}
