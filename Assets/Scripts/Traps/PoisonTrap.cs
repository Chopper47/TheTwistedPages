using System.Collections;
using UnityEngine;

public class PoisonTrap : Trap
{
    public GameObject poisonCloud;
    public override void AfterActivation()
    {

        poisonCloud.SetActive(true);
        StartCoroutine(DestroyCountdown());
    }

    IEnumerator DestroyCountdown()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(5f);

        Destroy(this.transform.parent.gameObject);
    }
}
