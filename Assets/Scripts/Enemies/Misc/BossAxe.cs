using System.Collections;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
    private SplineAnimate followPath;
    [SerializeField] private int damage = 1;
    [SerializeField] private KnotLinkCollection farthestPoint;
    [SerializeField] private LayerMask WallMask;

    private void Awake()
    {
        originalParent = transform.parent;
        ogTransform = transform;
        followPath = GetComponent<SplineAnimate>();
    }

    public void Update()
    {
        if(followPath.IsPlaying)
        {
            if(followPath.ElapsedTime>=0.9)
            {
                arrived = true;
            }
        }
    }

    public void SetLumberjack(Lumberjack newLumberjack)
    {
        lumberjack = newLumberjack;
    }

    public bool MovedByPlayer() { return playerGrabbed; }

    public void StartThrowAttackB(Vector3 startingPos, Vector3 throwDirection, float timeToOneUnit)
    {
        transform.parent = null;
        thrown = true;
        canGet = false;
        moving = StartCoroutine(Move(startingPos, throwDirection, timeToOneUnit));
        StartCoroutine(WaitUntilCollision());
    }

    public void StartThrowAttackA(float timeOnAir, float distance)
    {
        transform.parent = null;
        arrived = false;
        thrown = true;
        canGet = false;
        followPath.Duration = timeOnAir;
        SetFarthestPoint(distance);
        followPath.ElapsedTime = 0f;
        followPath.Play();
        StartCoroutine(WaitUntilCollision());
    }

    private void SetFarthestPoint(float distance)
    {
        var spline = followPath.Container.Spline;
        var farpoint = spline[1];
        farpoint.Position = new Vector3(farpoint.Position.x, farpoint.Position.y, distance);
        followPath.Container.Spline[1] = farpoint;
    }

    public void NewDirection(Vector3 startingPos, Vector3 throwDirection, float timeToOneUnit)
    {
        //StartCoroutine(Move(startingPos,new Vector3(lumberjack.transform.position.x - startingPos.x, 0f, lumberjack.transform.position.z - startingPos.z).normalized , timeOnAir));
        StartCoroutine(Move(startingPos, throwDirection, timeToOneUnit));
    }

    public void InterruptedThrowAttack()
    {
        if (moving != null)
        {
            StopCoroutine(moving);
        }
        if(followPath.IsPlaying)
        {
            followPath.Pause();
        }
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

    IEnumerator Move(Vector3 startingPos, Vector3 dir, float timeToOneUnit)
    {
        RaycastHit hit;
        Vector3 endPoint = Vector3.zero;

        Debug.Log("Direction - " + dir);
        Debug.DrawRay(startingPos, dir * 100, Color.yellow, 100f);
        if(Physics.Raycast(startingPos, dir, out hit ,100f, WallMask))
        {
            endPoint = hit.point - (hit.normal * -0.25f);
        }

        float timer = 0f;
        float maxTimer = timeToOneUnit * Vector3.Distance(startingPos, endPoint);

        while (timer < maxTimer)
        {
            transform.position = Vector3.Lerp(startingPos, endPoint, timer / maxTimer);
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
               RecoveredFromBoss(boss);
            }
            else
            {
                ThrownToBoss(boss);
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

    public void RecoveredFromBoss(Lumberjack boss)
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

    public void ThrownToBoss(Lumberjack boss)
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
