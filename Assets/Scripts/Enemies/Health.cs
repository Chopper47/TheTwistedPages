using UnityEngine;

public class Health : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
