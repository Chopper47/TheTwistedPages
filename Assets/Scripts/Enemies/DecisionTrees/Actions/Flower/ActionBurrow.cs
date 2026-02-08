using System.Data.Common;
using UnityEngine;

public class ActionBurrow : IAction
{
    [SerializeField] Flower flower;

    public void Awake()
    {
        if (flower == null)
        {
            flower = transform.root.GetComponent<Flower>();
        }
    }
    public override void Act()
    {
        Debug.LogWarning("Burrow - " + transform.parent.parent.parent.name);
        flower.SetBurrow(true);
    }
}
