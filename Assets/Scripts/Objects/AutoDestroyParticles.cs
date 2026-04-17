using UnityEngine;
using System.Collections;

public class AutoDestroyParticle : MonoBehaviour
{
    public float lifeTime = 5.0f;

    private ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private IEnumerator Start()
    {
        if (particleSystem == null)
        {
            Destroy(gameObject, lifeTime);
            yield break;
        }

        yield return new WaitForSeconds(lifeTime);

        particleSystem.Stop();

        yield return new WaitWhile(() => particleSystem.IsAlive(true));

        Destroy(gameObject);
    }
}
