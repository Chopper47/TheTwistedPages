using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DummyTarget : MonoBehaviour
{
    [SerializeField] private float duration = 5.0f;
    [SerializeField] private float affectRadius = 10.0f;

    [Header("VFX")]
    [SerializeField] private SpriteRenderer rangeImage;
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material flickerMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float startFlickeringInterval = 0.5f;
    [SerializeField] private float endFlickeringInterval = 0.02f;

    private Transform playerTransform;
    private List<ChaseAgent> affectedEnemies = new List<ChaseAgent>();


    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalMaterial = meshRenderer.material;

        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
        StartCoroutine(Flickering(duration));
        Collider[] hits = Physics.OverlapSphere(transform.position, affectRadius);
        foreach (Collider hit in hits)
        {
            ChaseAgent agent = hit.GetComponent<ChaseAgent>();
            if (agent != null)
            {
                agent.SetTarget(transform);
                agent.Chase();
                affectedEnemies.Add(agent);
            }
        }

        Invoke(nameof(RemoveDummy), duration);
    }

    private void RemoveDummy()
    {
        foreach (ChaseAgent agent in affectedEnemies)
        {
            if (agent != null)
            {
                agent.SetTarget(playerTransform);
                agent.Chase();
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator Flickering(float timeForExplotion)
    {
        float flickerDuration = timeForExplotion - (timeForExplotion / 4);
        yield return new WaitForSeconds(timeForExplotion / 4); // Initial delay before flickering starts
        float elapsed = 0f;
        bool isGlow = false;


        while (elapsed < flickerDuration)
        {
            // Interpolate interval (gets faster over time)
            float t = elapsed / flickerDuration;
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
}