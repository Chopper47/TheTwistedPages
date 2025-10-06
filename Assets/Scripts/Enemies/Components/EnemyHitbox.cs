using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] EnemyHealth enemyHp;
    [SerializeField] int dmg;

    private void Awake()
    {
        if(enemyHp == null) { enemyHp = GetComponent<EnemyHealth>(); }
    }

    public int ShowEnemyState()
    {
        return enemyHp.CurrentState();
    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if(player != null ) 
        {
            player.TakeDamage(dmg);
        }
    }
}
