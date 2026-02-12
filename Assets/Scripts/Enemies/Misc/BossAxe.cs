using System.Collections;
using UnityEngine;

public class BossAxe : MonoBehaviour
{
    private bool playerGrabbed = false;
    private Coroutine moving;
    [SerializeField] LayerMask WallMask;

    public bool MovedByPlayer() { return playerGrabbed; }

    public void StartThrowAttackA(Vector3 startingPos, Vector3 throwDirection, float timeOnAir)
    {
        moving = StartCoroutine(MoveInPlayerDirection(startingPos, throwDirection, timeOnAir));
    }

    public void InterruptedThowAttack()
    {
        StopCoroutine(moving);
        playerGrabbed = true;
    }

    public void ResetPlayerGrabbed()
    {
        playerGrabbed = false;
    }

    public void AxeRotationAnim()
    {
        //calls animator and sets transition varaible
    }

    IEnumerator MoveInPlayerDirection(Vector3 startingPos, Vector3 dir, float timeOnAir)
    {
        RaycastHit hit;
        Vector3 endPoint = Vector3.zero;

        if(Physics.Raycast(startingPos, dir, out hit ,100f, WallMask))
        {
            endPoint = hit.point - (dir * -0.5f);
        }

        float timer = 0f;

        while (timer < timeOnAir)
        {
            transform.position = Vector3.Lerp(startingPos, endPoint, timer / timeOnAir);
            timer += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
}
