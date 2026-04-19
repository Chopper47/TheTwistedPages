using System.Collections;
using UnityEngine;

public class DestructibleEnviroment : MonoBehaviour
{
    [SerializeField] private GameObject destructionParticles;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DestructionCheck") || other.CompareTag("BossHitbox"))
        {
            GameObject particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();
            Destroy(gameObject);
        }
    }

    
}
