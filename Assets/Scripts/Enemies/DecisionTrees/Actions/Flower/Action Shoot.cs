using UnityEngine;

public class ActionShoot : IAction
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
        flower.Shoot();
    }
}
