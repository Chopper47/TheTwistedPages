using System.Collections;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class BossAxe : MonoBehaviour
{
    private bool playerGrabbed = false;
    private bool thrown = false;
    private bool arrived = false;
    private bool canGet = false;
    private Coroutine moving;
    private Transform originalParent;
    private Transform ogTransform;
    private Lumberjack lumberjack;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask WallMask;

    private void Awake()
    {
        originalParent = transform.parent;
        ogTransform = transform;
    }

    public void SetLumberjack(Lumberjack newLumberjack)
    {
        lumberjack = newLumberjack;
    }

    public bool MovedByPlayer() { return playerGrabbed; }

    public void StartThrowAttackA(Vector3 startingPos, Vector3 throwDirection, float timeOnAir)
    {
        transform.parent = null;
        thrown = true;
        canGet = false;
        moving = StartCoroutine(Move(startingPos, throwDirection, timeOnAir));
        StartCoroutine(WaitUntilCollision());
    }

    public void NewDirection(Vector3 startingPos, Vector3 throwDirection, float timeOnAir)
    {
        StartCoroutine(Move(startingPos,new Vector3(lumberjack.transform.position.x - startingPos.x, 0f, lumberjack.transform.position.z - startingPos.z).normalized , timeOnAir));
        //StartCoroutine(Move(startingPos, throwDirection, timeOnAir));
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

    public bool IsAttacking()
    {
        return thrown && !playerGrabbed;
    }

    IEnumerator Move(Vector3 startingPos, Vector3 dir, float timeOnAir)
    {
        RaycastHit hit;
        Vector3 endPoint = Vector3.zero;

        if(Physics.Raycast(startingPos, dir, out hit ,100f, WallMask))
        {
            endPoint = hit.point - (hit.normal * -0.25f);
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
        lumberjack.CanGetAxe();
        transform.position = endPoint       ;
        yield return null;
    }
    
    IEnumerator WaitUntilCollision()
    {
        yield return new WaitForSeconds(0.5f);
        canGet = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Lumberjack boss = other.GetComponent<Lumberjack>();

        if (boss != null && canGet) 
        {
            if(arrived)
            {
                transform.parent = originalParent;
                transform.position = ogTransform.position;
                transform.rotation = ogTransform.rotation;
                transform.localScale = ogTransform.localScale;
                boss.PickUpAxe();
                arrived = false;
                playerGrabbed = false;
                gameObject.SetActive(false);
            }
            else
            {
                transform.parent = originalParent;
                transform.position = ogTransform.position;
                transform.rotation = ogTransform.rotation;
                transform.localScale = ogTransform.localScale;
                playerGrabbed = false;
                boss.RecieveAxe();
                StopCoroutine(moving);
                boss.PickUpAxe();
                gameObject.SetActive(false);
            }
        }

        if(thrown)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();

            if(player != null && !PlayerMovement.IsInvincible() && !playerGrabbed)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
