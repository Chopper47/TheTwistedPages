using System.Collections;
using UnityEngine;

public class PoisonTrap : Trap
{
    public GameObject poisonCloud;

    protected override void AfterActivation()
    {
        poisonCloud.SetActive(true);
        StartCoroutine(DestroyCountdown());
    }

    private IEnumerator DestroyCountdown()
    {
        GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(5.0f);

        Destroy(transform.parent.gameObject);
    }
}
