using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
