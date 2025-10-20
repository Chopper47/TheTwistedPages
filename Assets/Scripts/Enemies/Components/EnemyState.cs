using UnityEngine;

public class EnemyState : MonoBehaviour
{
    enum State
    {
        Stay,
        Chase,
        Attack
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

    public int CurrentState()
    {
        return (int)state;
    }
}
