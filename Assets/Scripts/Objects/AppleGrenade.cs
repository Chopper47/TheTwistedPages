using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AppleGrenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] float explosionDelay = 2.0f;
    [SerializeField] float randomDelayOffset = 0.4f;
    [SerializeField] float explosionRadius = 2.5f;
    [SerializeField] int explosionDamage = 1;

    [Header("VFX")]
    [SerializeField] private SpriteRenderer rangeImage;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material flickerMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float startFlickeringInterval = 0.2f;
    [SerializeField] private float endFlickeringInterval = 0.02f;
    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    [SerializeField] private AudioSource Main;


    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        float offset = Random.Range(0f, randomDelayOffset);

        StartCoroutine(ExplodeAfterDelay(explosionDelay + offset));
        StartCoroutine(Flickering(explosionDelay + offset));
        
        //StartCoroutine(VisualTimer(explosionDelay + offset));
    }
    private void Awake()
    {
        AM = FindAnyObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();
    }

    private IEnumerator Flickering(float timeForExplotion)
    {
        float elapsed = 0f;
        bool isGlow = false;

        while (elapsed < explosionDelay)
        {
            // Interpolate interval (gets faster over time)
            float t = elapsed / explosionDelay;
            Color lightColour = rangeImage.material.color;
            lightColour.a = Mathf.Lerp(0f, 5f, t);
            float currentInterval = Mathf.Lerp(startFlickeringInterval, endFlickeringInterval, t);

            // Swap material
            meshRenderer.material = isGlow ? originalMaterial : flickerMaterial;
            rangeImage.enabled = !isGlow;
            isGlow = !isGlow;

            yield return new WaitForSeconds(currentInterval);
            elapsed += currentInterval;
        }

        // Ensure final state (optional)
        meshRenderer.material = flickerMaterial;
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    private void Explode()
    {
        if (explosionParticles != null)
        {
            ParticleSystem explosion = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * explosionRadius;
            explosion.Play();
            AM.Play("ApplePop", null);
        }

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

        CameraShake.Shake(0.2f, 0.4f);
        Destroy(gameObject);
    }
}
