using UnityEngine;
using System.Collections;

public class Dummy : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifetime = 5.0f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 5.0f;
    [SerializeField] private int damage = 1;

    [Header("VFX")]
    [SerializeField] private SpriteRenderer rangeImage;
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material flickerMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float startFlickeringInterval = 0.5f;
    [SerializeField] private float endFlickeringInterval = 0.02f;

    private float timer;

    private void Start()
    {
        timer = lifetime;

        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        if (originalMaterial == null && meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
        }

        StartCoroutine(Flickering(lifetime));
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator Flickering(float totalTime)
    {
        float flickerDuration = totalTime - (totalTime / 4);
        yield return new WaitForSeconds(totalTime / 4);

        float elapsed = 0f;
        bool isGlow = false;

        while (elapsed < flickerDuration)
        {
            float t = elapsed / flickerDuration;
            float currentInterval = Mathf.Lerp(startFlickeringInterval, endFlickeringInterval, t);

            if (meshRenderer != null)
            {
                meshRenderer.material = isGlow ? originalMaterial : flickerMaterial;
            }

            if (rangeImage != null)
            {
                rangeImage.enabled = !isGlow;
            }

            isGlow = !isGlow;

            yield return new WaitForSeconds(currentInterval);
            elapsed += currentInterval;
        }

        if (meshRenderer != null)
        {
            meshRenderer.material = flickerMaterial;
        }
    }
}
