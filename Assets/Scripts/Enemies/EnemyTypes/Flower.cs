using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [Header("Burrow variables")]
    [SerializeField] private float maxBurrowedTime = 7f;
    [SerializeField] private float burrowedUnderVariation = .5f;
    [SerializeField] private int comeoutChance = 10;
    [SerializeField] private float timeForComeoutChance = .25f;
    private bool comeoutCheck = false;
    private bool checkTime = false;
    private bool maxTimePassed;
    private float timer = 0f;

    [Header("Non burrow variables")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private float shootCD = 1.5f;
    [SerializeField] private float burrowRange = 2f;
    [SerializeField] private float shootRange = 7f;
    [SerializeField] private int burrowChance = 10;
    [SerializeField] private float timeForBurrowChance = .25f;
    private bool onShootingCooldown = false;
    private bool burrowCheck = false;

    private bool checking = false;
    private bool burrowed = false;
    [Header("References")]
    [SerializeField] ChaseAgent chase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(checkTime)
        {
            timer += Time.deltaTime;

            if(timer > maxBurrowedTime)
            {
                maxTimePassed = true;
            }
        }
    }

    public bool IsBurrowed()
    {
        return burrowed;
    }

    public void SetBurrow(bool newBurrowed)
    {
        burrowCheck = false;
        comeoutCheck = false;
        chase.StopChase();

        burrowed = newBurrowed;

        if(burrowed)
        {
            checkTime = true;
            timer = 0f;
        }
        else
        {
            checkTime = false;
            maxTimePassed = false;
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

        if(i <= burrowChance)
        {
            burrowCheck = true;
        }
        burrowCheck = false;
        checking = false;
    }

    public void CallBurrow()
    {
        checking = true;
        StartCoroutine(TimerBurrow());
    }

    IEnumerator TimerBurrow()
    {
        yield return new WaitForSeconds(burrowChance);

        ChanceBurrow();
    }

    public bool BurrowCheck()
    { return burrowCheck; }

    #endregion

    #region ComeoutChance
    public void CallComeout()
    {
        checking = true;
        StartCoroutine(TimerComeout());
    }

    IEnumerator TimerComeout()
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
        checking = false;
    }

    public bool ComeoutCheck()
    { return comeoutCheck; }

    #endregion

    public bool CurrentlyChecking()
    {
        return checking;
    }

    public bool InShootRange()
    {
        return chase.DistanceFromTarget() <= shootRange;
    }

    public bool PlayerTooClose()
    {
        return (chase.DistanceFromTarget() <= burrowRange);
    }

    public void Shoot()
    {
        if (!onShootingCooldown)
        {
            onShootingCooldown = true;
            GameObject gO = Instantiate(projectile,transform.position, Quaternion.identity, null);
            gO.GetComponent<FlowerProjectile>().SetDirection(chase.TargetPosition() - transform.position);
            StartCoroutine(ShootCooldown());

        }
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCD);
        onShootingCooldown = false;
    }

    public bool IsUnderPlayer()
    {
        return Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(chase.TargetPosition().x, 0f, chase.TargetPosition().z)) <= burrowedUnderVariation ;
    }
}
