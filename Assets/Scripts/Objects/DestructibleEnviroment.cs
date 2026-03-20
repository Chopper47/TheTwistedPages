using UnityEngine;

public class DestructibleEnviroment : MonoBehaviour
{
    [SerializeField] private GameObject destructionParticles;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DestructionCheck") || other.CompareTag("BossHitbox"))
        {
            Instantiate(destructionParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
