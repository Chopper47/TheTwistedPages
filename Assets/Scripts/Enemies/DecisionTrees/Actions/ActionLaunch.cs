using UnityEngine;

public class ActionLaunch : IAction
{
    [SerializeField] ChaseAgent enemy;
    [SerializeField] WolfIA wolf;
    public override void Act()
    {
        if(!wolf.IsLaunching())
        {
            //Stops chase
            enemy.StopChase();

            //Starts preparing for launch
            wolf.SetLaunch(true);
            Debug.Log("start launch " + " - " + gameObject.name);
        }
    }
}
