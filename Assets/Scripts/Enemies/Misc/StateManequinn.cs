using UnityEngine;

public class StateManequinn : EnemyState
{
    enum State
    {
        Stay,
        Chase,
        Attack
    }

    [SerializeField]State test = State.Stay;
}
