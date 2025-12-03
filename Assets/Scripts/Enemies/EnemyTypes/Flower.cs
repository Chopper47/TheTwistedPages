using System.Collections;
using UnityEditor;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private float shootCD = 1.5f;
    [SerializeField] private float shootRange = 7.0f;
    [SerializeField] private float burrowRange = 2.0f;
    [SerializeField] private float burrowCooldown = 3.0f;
    [SerializeField] private float timeForChaseStart = 1.5f;
    [SerializeField] private float timeForBurrowChance = 0.25f;
    [SerializeField] private int burrowChance = 10;
    [SerializeField] private int countForTripleShoot = 3;

    private int shotCount = 0;
    private bool onShootingCooldown = false;
    private bool changeCooldown = false;

    [Header("Burrowing")]
    [SerializeField] private float maxBurrowedTime = 7.0f;
    [SerializeField] private float burrowedUnderVariation = 0.5f;
    [SerializeField] private int comeoutChance = 10;
    [SerializeField] private float timeForComeoutChance = 0.25f;

    private bool comeoutCheck = false;
    private bool checkTime = false;
    private bool maxTimePassed;
    private float timer = 0f;

    private bool burrowOnCooldown = false;
    private bool burrowCheck = false;
    private bool burrowed = false;
    private bool checkingChance = false;

    [Header("References")]
    [SerializeField] private ChaseAgent chase;
    [SerializeField] private Animator animator;

    private void Update()
    {
        if (checkTime)
        {
            timer += Time.deltaTime;

            if (timer > maxBurrowedTime)
            {
                maxTimePassed = true;
            }
        }
    }

    public bool IsBurrowInCooldown()
    {
        return burrowOnCooldown;
    }

    public bool IsBurrowed()
    {
        return burrowed;
    }

    public void StartBurrowCooldown()
    {
        burrowOnCooldown = true;
        StartCoroutine(BurrowCooldown());
    }

    private IEnumerator BurrowCooldown()
    {
        yield return new WaitForSeconds(burrowCooldown);
        burrowOnCooldown = false;
    }

    private IEnumerator ChangeCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        changeCooldown = false;
    }

    public void SetBurrow(bool newBurrowed)
    {
        if (!changeCooldown)
        {
            changeCooldown = true;
            burrowCheck = false;
            comeoutCheck = false;
            chase.StopChase();

            burrowed = newBurrowed;

            if (burrowed)
            {
                checkTime = true;
                timer = 0f;
                //Cambio visual provisional
                StartCoroutine(BurrowAnim(burrowed));
            }
            else
            {
                checkTime = false;
                maxTimePassed = false;
                //Cambio visual provisional
                StartCoroutine(BurrowAnim(burrowed));
            }
            StartCoroutine(ChangeCooldown());
        }
    }

    private IEnumerator BurrowAnim(bool isBurrowing)
    {
        if (isBurrowing)
        {
            float time = 0f;
            Vector3 startingPos = transform.position;
            Vector3 endingPos = startingPos - Vector3.up;
            while (time < timeForChaseStart)
            {
                transform.position = Vector3.Lerp(startingPos, endingPos, time/timeForChaseStart);
                time+=Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            float time = 0f;
            Vector3 startingPos = transform.position;
            Vector3 endingPos = startingPos + Vector3.up;
            while (time < timeForChaseStart)
            {
                transform.position = Vector3.Lerp(startingPos, endingPos, time / timeForChaseStart);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }

    public bool TimePassed()
    {
        return maxTimePassed;
    }

    #region BurrowChance
    public void ChanceBurrow()
    {
        int i = Random.Range(1, 101);

        if (i <= burrowChance)
        {
            burrowCheck = true;
        }
        burrowCheck = false;
        checkingChance = false;
    }

    public void CallBurrow()
    {
        checkingChance = true;
        StartCoroutine(TimerBurrow());
    }

    IEnumerator TimerBurrow()
    {
        yield return new WaitForSeconds(timeForBurrowChance);

        ChanceBurrow();
    }

    public bool BurrowCheck()
    { 
        return burrowCheck; 
    }

    #endregion

    #region ComeoutChance
    public void CallComeout()
    {
        checkingChance = true;
        StartCoroutine(TimerComeout());
    }

    private IEnumerator TimerComeout()
    {
        yield return new WaitForSeconds(timeForComeoutChance);

        ChanceComeOut();
    }

    public void ChanceComeOut()
    {
        int i = Random.Range(1, 101);

        if (i <= comeoutChance)
        {
            comeoutCheck = true;
        }
        comeoutCheck = false;
        checkingChance = false;
    }

    public bool ComeoutCheck()
    { 
        return comeoutCheck; 
    }

    #endregion

    public bool CurrentlyChecking()
    {
        return checkingChance;
    }

    public bool InShootRange()
    {
        return chase.DistanceFromTarget() <= shootRange;
    }

    public bool PlayerTooClose()
    {
        return (chase.DistanceFromTarget() <= burrowRange);
    }

    #region ShootingMethods
    public void Shoot()
    {
        if (!onShootingCooldown)
        {
            shotCount++;
            animator.SetBool("isShooting", true);

            if (shotCount == countForTripleShoot) 
            { 
                shotCount = 0;
                onShootingCooldown = true;
                Vector3 straightDir = chase.TargetPosition() - transform.position;
                straightDir.y = 0f;

                GameObject straightBullet = Instantiate(projectile, transform.position, Quaternion.identity, null);
                GameObject leftBullet = Instantiate(projectile, transform.position, Quaternion.identity, null);
                GameObject rightBullet = Instantiate(projectile, transform.position, Quaternion.identity, null);

                straightBullet.GetComponent<FlowerProjectile>().SetDirection(straightDir);
                leftBullet.GetComponent<FlowerProjectile>().SetDirection(Quaternion.AngleAxis(-30, Vector3.up) * straightDir);
                rightBullet.GetComponent<FlowerProjectile>().SetDirection(Quaternion.AngleAxis(30, Vector3.up) * straightDir);
            }
            else
            {
                onShootingCooldown = true;
                Vector3 straightDir = chase.TargetPosition() - transform.position;

                GameObject straightBullet = Instantiate(projectile, transform.position, Quaternion.identity, null);
                straightBullet.GetComponent<FlowerProjectile>().SetDirection(straightDir);
            }
            StartCoroutine(ShootCooldown());
        }
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCD-0.25f);
        animator.SetBool("isShooting", false);
        yield return new WaitForSeconds(0.25f);
        onShootingCooldown = false;
    }
    #endregion

    public bool IsUnderPlayer()
    {
        return Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(chase.TargetPosition().x, 0f, chase.TargetPosition().z)) <= burrowedUnderVariation;
    }

    public void StartChaseDelay()
    {
        chase.SetStartChase(false);
        StartCoroutine(ChaseDelay());   
    }

    private IEnumerator ChaseDelay()
    {
        yield return new WaitForSeconds(timeForChaseStart);
        chase.SetStartChase(true);
    }
}
