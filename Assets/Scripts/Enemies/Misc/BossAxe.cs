using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossAxe : MonoBehaviour
{
    private bool playerGrabbed = false;
    private bool thrown = false;
    private bool arrived = false;
    private Coroutine moving;
    private Transform originalParent;
    private Transform ogTransform;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask WallMask;

    private void Awake()
    {
        originalParent = transform.parent;
        ogTransform = transform;
    }

    public bool MovedByPlayer() { return playerGrabbed; }

    public void StartThrowAttackA(Vector3 startingPos, Vector3 throwDirection, float timeOnAir)
    {
        transform.parent = null;
        thrown = true;
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

        thrown = false;
        arrived = true;
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Lumberjack boss = other.GetComponent<Lumberjack>();

        if (boss != null && arrived) 
        {
            transform.parent = originalParent;
            transform.position = ogTransform.position;
            transform.rotation = ogTransform.rotation;
            transform.localScale = ogTransform.localScale;
            boss.PickUpAxe();
            arrived = false;
            gameObject.SetActive(false);
        }

        if(thrown)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();

            if(player)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
