using UnityEngine;

public class ActionReceiveAxe : IAction
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
        lumberjack.FalseReceiveAxe();
        Debug.Log("Lumberjack: Receive Axe");
    }
}
