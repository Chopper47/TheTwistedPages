using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] EnemyHealth enemyHp;
    [SerializeField] EnemyState states;
    [SerializeField] int dmg;

    private void Awake()
    {
        if(enemyHp == null) { enemyHp = GetComponent<EnemyHealth>(); }
        if(states == null) { states = GetComponent<EnemyState>(); }
    }

    public int ShowEnemyState()
    {
        return states.CurrentState();
    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if(player != null && !PlayerMovement.IsInvincible()) 
        {
            CameraShake.Shake(0.5f, 0.5f);
            player.TakeDamage(dmg);
        }
    }
}
