using UnityEngine;
using System.Collections;

public class AutoDestroyParticle : MonoBehaviour
{
    public float lifeTime = 5.0f;

    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private IEnumerator Start()
    {
        if (ps == null)
        {
            Destroy(gameObject, lifeTime);
            yield break;
        }

        yield return new WaitForSeconds(lifeTime);

        ps.Stop();

        yield return new WaitWhile(() => ps.IsAlive(true));

        Destroy(gameObject);
    }
}
