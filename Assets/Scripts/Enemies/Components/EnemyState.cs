using UnityEngine;

enum State
{
    Stay,
    Chase,
    Attack
}

public class EnemyState : MonoBehaviour
{
    public static EnemyState Instance;

    State state = State.Stay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
