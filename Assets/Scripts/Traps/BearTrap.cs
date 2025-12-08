using System.Collections;
using UnityEngine;

public class BearTrap : Trap
{
    [SerializeField] private float destroyDelay = 1.5f;

    protected override void AfterActivation()
    {
        StartCoroutine(DestroyCountdown());
    }

    private IEnumerator DestroyCountdown()
    {
        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}
