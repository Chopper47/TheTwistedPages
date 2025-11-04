using UnityEngine;

public class ActionStepBack : IAction
{
    [SerializeField] Wolf wolf;

    private void Awake()
    {
        if(!wolf)
        {
            wolf = transform.root.GetComponent<Wolf>();
        }
    }

    public override void Act()
    {

        if(!wolf.CanStepback())
        {
            Debug.Log("SteppingBack");
            wolf.SetStepback(true);
        }
    }
}
