using UnityEngine;

public class EnemyState : MonoBehaviour
{
    enum State
    {
        Stay,
        Chase,
        Attack,
        Pushed
    }

    public static EnemyState Instance;

    State state = State.Stay;

    public void AttackState()
    {
        state = State.Attack;
    }

    public void StayState()
    {
        state = State.Stay;
    }

    public void ChaseState()
    {
        state = State.Chase;
    }

    public void PushedState()
    {
        state = State.Pushed;
    }

    public int CurrentState()
    {
        return (int)state;
    }

    public bool IsBeingPushed()
    {
        return state == State.Pushed;
    }
}
