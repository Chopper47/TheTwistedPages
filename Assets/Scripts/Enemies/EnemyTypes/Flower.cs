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
    [SerializeField] private float timeForShootingStart = 1.5f;
    [SerializeField] private float timeForBurrowChance = 0.25f;
    [SerializeField] private float shootingAnimationDelay = 0.1f;
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
    private bool burrowAnimation = false;
    private bool checkingChance = false;

    [Header("References")]
    [SerializeField] private ChaseAgent chase;
    [SerializeField] private Animator animator;

    [Header("Particle references")]
    [SerializeField] private Transform groundParticlePosition;
    [SerializeField] private ParticleSystem groundParticle;
    private CapsuleCollider hitbox;

    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    public AudioSource Main;

    private void Awake()
    {
        hitbox = GetComponent<CapsuleCollider>();
    }

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
        burrowAnimation = true;
        if (isBurrowing)
        {
            animator.SetBool("isBurrowing", true);
            float timer = 0f;

            while (timer < timeForChaseStart)
            {
                hitbox.center = Vector3.Lerp(new Vector3(0,0.75f,0), new Vector3(0,-1.1f,0), timer/timeForChaseStart);
                timer += Time.deltaTime;
                yield return null;
            }
            burrowAnimation = false;
            //groundParticle.transform.localPosition = Vector3.up * 1.2f;
            animator.SetBool("isBurrowing", false);
            groundParticle.Play();
        }
        else
        {
            animator.SetBool("isComingOut", true);

            float timer = 0f;

            while (timer < timeForChaseStart)
            {
                hitbox.center = Vector3.Lerp(new Vector3(0, -1.1f, 0), new Vector3(0, 0.75f, 0), timer / timeForChaseStart);
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(timeForShootingStart);
            //groundParticle.transform.localPosition = Vector3.zero;
            burrowAnimation = false;
            animator.SetBool("isComingOut", false);
            groundParticle.Stop();
        }
        yield return null;
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
    public void StartShoot()
    {
        if (!onShootingCooldown && !burrowAnimation)
        {
            animator.SetBool("isShooting", true);

            onShootingCooldown = true;
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(shootingAnimationDelay);
        shotCount++;
        AM.Play("Shoot", Main);

        Vector3 straightDir = chase.TargetPosition() - transform.position;

        if (shotCount == countForTripleShoot)
        {
            shotCount = 0;
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

            GameObject straightBullet = Instantiate(projectile, transform.position, Quaternion.identity, null);
            straightBullet.GetComponent<FlowerProjectile>().SetDirection(straightDir);
        }
        StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCD - shootingAnimationDelay);
        animator.SetBool("isShooting", false);
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
