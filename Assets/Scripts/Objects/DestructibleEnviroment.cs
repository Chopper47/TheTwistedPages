using UnityEngine;

public class DestructibleEnviroment : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DestructionCheck") || other.CompareTag("BossHitbox"))
        {
            Destroy(gameObject);
        }
    }
}
