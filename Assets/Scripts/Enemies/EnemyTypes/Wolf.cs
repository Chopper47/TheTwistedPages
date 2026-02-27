using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Wolf : CollisionRefresh 
{
    [Header("Condition variables")]
    [SerializeField] float minStepbackDistance = 0.5f;
    [SerializeField] float maxStepbackDistance = 3.5f;
    [SerializeField] float launchRange = 4f;
    [SerializeField] int consecutiveStepback = 0;
    [SerializeField] bool howling = false;

    [Header("Launch variables")]
    [SerializeField] float timeForLaunch = .8f;
    [SerializeField] float launchDistance = 5f;
    [SerializeField] float launchCD = 1.5f;
    //Acceleration and deceleration values
    [SerializeField] float launchDuration = .6f;
    [SerializeField] float launchAcceleratePercentage = .1f;
    
    [SerializeField] float launchConstantPercentage = .85f;

    [SerializeField] float launchDeceleratePercentage = .05f;
    bool launch = false;
    bool canLaunch = true;

    [Header("Stepback variables")]
    [SerializeField] float timeForStepback = 0f;
    //Distance of the stepback, never too similar to launchDistance
    [SerializeField] float stepbackDistance = 3.5f;
    [SerializeField] float stepbackCD = 3.5f;

    [SerializeField] float stepbackDuration = 0.5f;
    [SerializeField] float stepbackAcceleratePercentage = .1f;

    [SerializeField] float stepbackConstantPercentage = .85f;
    
    [SerializeField] float stepbackDeceleratePercentage = .05f;

    [SerializeField] float stepbackTimeConsecutive = 0.75f;
    [SerializeField] int maxConsecutive = 2;
    bool checkConsecutiveStepback = false;
    Coroutine checkCoroutine;
    Coroutine launchCoroutine;
    bool stepback = false;
    bool canStepback = true;

    Vector3 launchStartingPos;
    Vector3 stepbackStartingPos;
    Vector3 launchDir;

    [Header("General Variables")]

    [SerializeField] float raycastOffset = 0.7f;
    [SerializeField] float howlTime = 0.7f;
    [SerializeField] LayerMask mask;
    [SerializeField] LayerMask triggerMask;

    Rigidbody rb;
    EnemyHealth enemyHealth;
    EnemyState states;
    [SerializeField] WolfAnimations animations;
    [SerializeField] ChaseAgent chase;


    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    public AudioSource Main;
    /// <summary>
    /// Called at the start for getting the Rigidbody component
    /// </summary>
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();
        states = GetComponent<EnemyState>();
        Main=GetComponentInChildren<AudioSource>();
        AM=FindFirstObjectByType<AudioManager>();
        StartCoroutine(Panting());
    }

    /// <summary>
    /// In the update it checks if the distance it has done while doing launching, when it has stops doing the launch
    /// </summary>
    public void Update()
    {
        if(chase.IsMoving())
        {
            //animations.TriggerWalk(true);
        }
        else
        {
            //animations.TriggerWalk(false);
        }
    }

    public void SetLaunch(bool launching)
    {
        launch = launching;
        Debug.Log("Launch set to " + launch);
        if(launch) 
        {
            canLaunch = false;
            if(checkConsecutiveStepback)
            {
                //UnityEngine.Debug.Log("revenge");
                StopCoroutine(checkCoroutine);
                consecutiveStepback = 0;
                checkConsecutiveStepback = false;
            }

            states.AttackState();
            launchCoroutine = StartCoroutine(WaitForLaunch());
        }
    }

    public void SetStepback(bool isStepback)
    {
        stepback = isStepback;

        if(stepback)
        {
            states.ChaseState();
            canStepback = false;
            StartCoroutine(WaitForStepback());
        }
    }

    public bool CanLaunch()
    {
        return canLaunch;
    }

    public bool CanStepback()
    { 
        return canStepback; 
    }

    public bool IsLaunching()
    {
        return launch;
    }

    public bool IsSteppingBack()
    {
        return stepback;
    }

    public bool IsHowling()
    {
        return howling;
    }

    public void Howl()
    {
        howling = true;
        //animations.SetHowling(true);
        StartCoroutine(HowlTimer());
    }

    IEnumerator HowlTimer()
    {
        yield return new WaitForSeconds(howlTime);
        howling = false;
        animations.TriggerStepback();
    }

    IEnumerator WaitForStepback()
    {
        //animations.SetWalking();

        yield return new WaitForSeconds(timeForStepback);

        transform.LookAt(new Vector3(chase.TargetPosition().x,transform.position.y, chase.TargetPosition().z));

        Vector3 sbDir = (new Vector3(transform.position.x - chase.TargetPosition().x, 0f, transform.position.z - chase.TargetPosition().z)).normalized;

        stepbackStartingPos = transform.position;

        Vector3 stepbackEndingPos;

        if (Physics.Raycast(transform.position ,sbDir, out RaycastHit hit, stepbackDistance + raycastOffset, mask))
        {
            Debug.LogWarning("Found wall in stepback");
            stepbackEndingPos = transform.position + sbDir * (Vector3.Distance(transform.position, hit.point) - raycastOffset);
        }
        else
        {
            Debug.LogWarning("Not found wall in stepback");
            stepbackEndingPos = transform.position + sbDir * stepbackDistance;
        }

        //Debug.LogWarning("Accelerate");
        
        //animations.SetStepback(true);
        float time = 0;                                                                                   //Acceleration

        while (time < stepbackDuration * stepbackAcceleratePercentage)
        {
            transform.position = Vector3.Lerp(stepbackStartingPos, stepbackStartingPos + sbDir * Vector3.Distance(stepbackStartingPos, stepbackEndingPos) * stepbackAcceleratePercentage, time/ stepbackDuration * stepbackAcceleratePercentage);
            time += Time.deltaTime;
            yield return null;
        }

        //Debug.LogWarning("Constant");
        time = 0;
        Vector3 currentPos = transform.position;
        while (time < stepbackDuration * stepbackConstantPercentage)                                                                       //Time in constant speed
        {
            transform.position = Vector3.Lerp(currentPos, currentPos + sbDir * Vector3.Distance(stepbackStartingPos, stepbackEndingPos) * stepbackConstantPercentage, time/ stepbackDuration * stepbackConstantPercentage);
            time += Time.deltaTime;
            yield return null;
        }

        //Debug.LogWarning("Decelerate");
        time = 0;
        currentPos = transform.position;

        while (time < stepbackDuration * stepbackDeceleratePercentage)                                                                       //Deceleration
        {
            transform.position = Vector3.Lerp(currentPos, stepbackEndingPos, time/ stepbackDuration * stepbackDeceleratePercentage);
            time += Time.deltaTime;
            yield return null;
        }
        //rb.AddForce(sbDir * stepbackForce);

        //animations.SetStepback(false);
        StopStepback();
        if (!checkConsecutiveStepback)
        {
            checkConsecutiveStepback = true;
            checkCoroutine = StartCoroutine(StopCheckStepback());
        }
        else
        {
            consecutiveStepback++;
            StopCoroutine(checkCoroutine);
            checkCoroutine = StartCoroutine(StopCheckStepback());
        }
    }

    IEnumerator StopCheckStepback()
    {
        yield return new WaitForSeconds(stepbackTimeConsecutive);
        consecutiveStepback = 0;
        checkConsecutiveStepback = false;
    }

    IEnumerator WaitForLaunch()
    {
        //animations.TriggerWalk(false);
        yield return new WaitForSeconds(timeForLaunch);

        transform.LookAt(new Vector3(chase.TargetPosition().x, transform.position.y, chase.TargetPosition().z));

        Vector3 launchDir = (new Vector3(chase.TargetPosition().x - transform.position.x, 0f, chase.TargetPosition().z - transform.position.z)).normalized;

        launchStartingPos = transform.position;

        Vector3 launchEndingPos;

        if (Physics.Raycast(transform.position, launchDir, out RaycastHit hit, launchDistance + raycastOffset, mask))
        {
            //Debug.LogWarning("Found wall in launch");

            float raycastOffsetApplied = raycastOffset;

            if(Physics.Raycast(transform.position, launchDir, out RaycastHit triggerHit, launchDistance + raycastOffset, triggerMask))
            {
                raycastOffsetApplied = 0f;
            }

            launchEndingPos = transform.position + launchDir * (Vector3.Distance(transform.position, hit.point) - raycastOffsetApplied);
        }
        else
        {
            //Debug.LogWarning("Not found wall in launch");
            launchEndingPos = transform.position + launchDir * launchDistance;
        }

        //animations.SetAttacking(true);
        
        float time = 0;                                                                                   //Acceleration
        StopCoroutine(Panting());
        AM.Play("Growl", Main);
        while (time < launchDuration * launchAcceleratePercentage)
        {
            if(states.IsBeingPushed())
            {
                //animations.SetAttacking(false);
                StopLaunch();
                StopCoroutine(launchCoroutine);
            }
            transform.position = Vector3.Lerp(launchStartingPos, launchStartingPos + launchDir * Vector3.Distance(launchStartingPos, launchEndingPos) * launchAcceleratePercentage , time/ launchDuration * launchAcceleratePercentage);
            time += Time.deltaTime;
            yield return null;
        }

        Vector3 currentPos = transform.position;
        time = 0;

        while (time < launchDuration * launchConstantPercentage)                                                                       //Time in constant speed
        {
            if (states.IsBeingPushed())
            {
                //animations.SetAttacking(false);
                StopLaunch();
                StopCoroutine(launchCoroutine);
            }
            transform.position = Vector3.Lerp(currentPos, currentPos + launchDir * Vector3.Distance(launchStartingPos, launchEndingPos) * launchConstantPercentage, time/ launchDuration * launchConstantPercentage);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        currentPos = transform.position;

        while (time < launchDuration * launchDeceleratePercentage)                                                                       //Deceleration
        {
            if (states.IsBeingPushed())
            {
                //animations.SetAttacking(false);
                StopLaunch();
                StopCoroutine(launchCoroutine);
            }
            transform.position = Vector3.Lerp(currentPos, launchEndingPos, time/ launchDuration * launchDeceleratePercentage);
            time += Time.deltaTime;
            yield return null;
        }

        consecutiveStepback = 0;

        //animations.SetAttacking(false);

        StopLaunch();
    }


    /// <summary>
    /// Cooldown for the launch
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeForLaunchReset()
    {
        yield return new WaitForSeconds(launchCD);
        canLaunch = true;
    }

    IEnumerator TimeForStepbackReset()
    {
        yield return new WaitForSeconds(stepbackCD);
        canStepback = true;
    }

    /// <summary>
    /// Reset function for the launch
    /// </summary>
    public void StopLaunch()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        launch = false;
        StartCoroutine(TimeForLaunchReset());
        StartCoroutine(Panting());
    }

    public void StopStepback()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        stepback = false;
        StartCoroutine(TimeForStepbackReset());
    }

    public float MinDistanceStepback()
    {
        return minStepbackDistance;
    }

    public float MaxDistanceStepback()
    {
        return maxStepbackDistance;
    }


    public float LaunchRange()
    {
        return launchRange;
    }

    public bool ConsectiveLimit()
    {
        return consecutiveStepback == maxConsecutive;
    }
    /// <summary>
    /// Calls the method for reset if it colides with a wall
    /// </summary>
    public override void OnWallHit()
    {
        if(states.IsBeingPushed())
        {
            states.StayState();
        }

        if(launch)
        {
            StopLaunch();
        }

        if(stepback) 
        {
            StopStepback();
        }
    }


    /// <summary>
    /// Calls the method for reset if it colides with the player
    /// </summary>
    public override void OnPlayerHit()
    {
        if (launch)
        {
            StopLaunch();
        }

        if (stepback)
        {
            StopStepback();
        }
    }
    public override void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            OnPlayerHit();
        }

        if (other.CompareTag("Wall"))
        {
            OnWallHit();
            //UnityEngine.Debug.Log("hit");
        }
    }

    private IEnumerator Panting()
    {
        while(true) 
        {
            AM.Play("Pant", Main);
            yield return new WaitForSeconds(5f);
        }
    }
}
