using System.Collections;
using UnityEngine;

public class PoisonTrap : Trap
{
    public GameObject poisonCloud;
    public GameObject model;
    public AudioManager AM;
    public AudioSource Main;

    private void Awake()
    {
        AM = FindFirstObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();
    }

    protected override void AfterActivation()
    {
        poisonCloud.SetActive(true);
        AM.Play("Gas", Main);
        StartCoroutine(DestroyCountdown());
    }

    private IEnumerator DestroyCountdown()
    {
        model.SetActive(false);

        yield return new WaitForSeconds(5.0f);

        Destroy(transform.parent.gameObject);
    }
}
