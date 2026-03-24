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
    [SerializeField] private Image timerImage;

    private void Start()
    {
        float offset = Random.Range(0f, randomDelayOffset);

        StartCoroutine(ExplodeAfterDelay(explosionDelay + offset));
        StartCoroutine(VisualTimer(explosionDelay + offset));
        rangeImage.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1);
        timerImage.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1);
    }

    private IEnumerator VisualTimer(float timeForExplotion)
    {
        rangeImage.gameObject.SetActive(true);
        timerImage.gameObject.SetActive(true);
        float time = 0f;
        while (time < timeForExplotion)
        {
            timerImage.fillAmount = Mathf.Lerp(1,0, time/timeForExplotion);
            time += Time.deltaTime;
            yield return null;
        }
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

        CameraShake.Shake(0.2f, 0.4f);
        Destroy(gameObject);
    }
}
