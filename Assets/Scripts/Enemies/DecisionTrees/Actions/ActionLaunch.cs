using UnityEngine;

public class ActionLaunch : IAction
{
    [SerializeField] ChaseAgent enemy;
    [SerializeField] WolfAI wolf;

    /// <summary>
    /// Checks if wolf is currently lauching, if its not launching stops chasing and calls the Lunch method from wolf
    /// </summary>
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
