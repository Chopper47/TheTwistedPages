using UnityEngine;

public class ActionComeOut : IAction
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
        Debug.LogWarning("Come out  - "+transform.parent.name);
        flower.SetBurrow(false);
    }
}
