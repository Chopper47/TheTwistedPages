using System.Collections;
using UnityEngine;

public class BearTrap : Trap
{
    protected override void AfterActivation()
    {
        StartCoroutine(DestroyCountdown());
    }

    private IEnumerator DestroyCountdown()
    {
        yield return new WaitForSeconds(3.0f);

        Destroy(gameObject);
    }
}
