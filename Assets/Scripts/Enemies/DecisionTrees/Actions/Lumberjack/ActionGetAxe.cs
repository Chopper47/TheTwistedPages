using UnityEngine;

public class ActionGetAxe : IAction
{
    [SerializeField] private Lumberjack lumberjack;

    private void Awake()
    {
        if (!lumberjack)
        {
            lumberjack = transform.root.GetComponent<Lumberjack>();
        }
    }

    public override void Act()
    {
        Debug.Log("Lumberjack: Get Axe");
        lumberjack.GetAxe();
    }
}
