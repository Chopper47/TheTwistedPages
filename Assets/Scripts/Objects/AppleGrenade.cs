using UnityEngine;
using System.Collections;

public class AppleGrenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] float explosionDelay = 2.0f;
    [SerializeField] float randomDelayOffset = 0.4f;
    [SerializeField] float explosionRadius = 2.5f;
    [SerializeField] int explosionDamage = 1;

    private void Start()
    {
        float offset = Random.Range(0f, randomDelayOffset);
        StartCoroutine(ExplodeAfterDelay(explosionDelay + offset));
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth player = hit.gameObject.GetComponent<PlayerHealth>();
                player.TakeDamage(explosionDamage);
                Debug.Log("Player was hit by an apple grenade.");
            }
        }

        Destroy(gameObject);
    }
}
