using UnityEngine;

public class DestructibleEnviroment : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DestructionCheck"))
        {
            Destroy(gameObject);
        }
    }
}
