using UnityEngine;

public class Health : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap") || other.CompareTag("Hole"))
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
