using System;
using UnityEngine;
using UnityEngine.Events;

public class EventAction : IAction
{
    public UnityEvent methodCalled;

    public override void Act()
    {
        Debug.Log(transform.name + " called");
        methodCalled.Invoke();
    }
}
