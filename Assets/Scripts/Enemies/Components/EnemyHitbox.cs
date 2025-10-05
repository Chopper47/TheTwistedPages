using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] int dmg;

    public void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if(player != null ) 
        {
            player.TakeDamage(dmg);
        }
    }
}
