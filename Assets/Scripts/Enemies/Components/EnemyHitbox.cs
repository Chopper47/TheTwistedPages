using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] EnemyHealth enemyHp;
    [SerializeField] EnemyState states;
    [SerializeField] int dmg;

    private bool ignorePlayer = false;
    private void Awake()
    {
        if(enemyHp == null) { enemyHp = GetComponent<EnemyHealth>(); }
        if(states == null) { states = GetComponent<EnemyState>(); }
    }

    public int ShowEnemyState()
    {
        return states.CurrentState();
    }

    public void SetIgnore(bool newIgnorePlayer)
    {
        ignorePlayer = newIgnorePlayer;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(ignorePlayer) { return; }
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if(player != null && !PlayerMovement.IsInvincible()) 
        {
            CameraShake.Shake(0.5f, 0.5f);
            player.TakeDamage(dmg);
        }

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if(playerMovement != null && playerMovement.CanPush(this.gameObject))
        {

        }
    }
}
