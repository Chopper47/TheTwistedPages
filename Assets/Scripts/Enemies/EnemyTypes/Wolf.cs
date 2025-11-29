using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Wolf : CollisionRefresh 
{
    [Header("Condition variables")]
    [SerializeField] float minStepbackDistance = 0.5f;
    [SerializeField] float maxStepbackDistance = 3.5f;
    [SerializeField] float launchRange = 4f;
    [SerializeField] int consecutiveStepback = 0;

    [Header("Launch variables")]
    [SerializeField] float timeForLaunch = .8f;
    [SerializeField] float launchDistance = 5f;
    [SerializeField] float launchForce = 2f;
    [SerializeField] float launchCD = 1.5f;
    //Acceleration and deceleration values
    [SerializeField] float launchSpeed = 36;
    [SerializeField] float launchAccelerateTime = .125f;
    [SerializeField] float launchConstantTime = .258f;
    [SerializeField] float launchDecelerateTime = .05f;
    bool launch = false;
    bool canLaunch = true;
    bool checkLaunchDistance = false;

    [Header("Stepback variables")]
    [SerializeField] float timeForStepback = 0f;
    //Distance of the stepback, never too similar to launchDistance
    [SerializeField] float stepbackDistance = 3.5f;
    [SerializeField] float stepbackForce = 2f;
    [SerializeField] float stepbackCD = 3.5f;

    [SerializeField] float stepbackSpeed = 36;
    [SerializeField] float stepbackAccelerateTime = .125f;
    [SerializeField] float stepbackConstantTime = .258f;
    [SerializeField] float stepbackDecelerateTime = .05f;

    [SerializeField] float stepbackTimeConsecutive = 0.75f;
    [SerializeField] int maxConsecutive = 2;
    bool checkConsecutiveStepback = false;
    Coroutine checkCoroutine;
    bool stepback = false;
    bool canStepback = true;
    bool checkStepbackDistance = false;

    Vector3 launchStartingPos;
    Vector3 stepbackStartingPos;
    Vector3 launchDir;

    Rigidbody rb;
    EnemyHealth enemyHealth;
    EnemyState states;
    [SerializeField] ChaseAgent chase;

    /// <summary>
    /// Called at the start for getting the Rigidbody component
    /// </summary>
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();
        states = GetComponent<EnemyState>();
    }

    /// <summary>
    /// In the update it checks if the distance it has done while doing launching, when it has stops doing the launch
    /// </summary>
    public void Update()
    {
        if (checkLaunchDistance) 
        { 
            if(Vector3.Distance(launchStartingPos, transform.position) >= launchDistance)
            {
                //UnityEngine.Debug.Log(rb.linearVelocity.magnitude);
                StopLaunch();
            }
        }

        if(checkStepbackDistance) 
        { 
            if(Vector3.Distance(stepbackStartingPos, transform.position) >=  stepbackDistance)
            {
                StopStepback();
            }
        }
    }

    public void SetLaunch(bool launching)
    {
        launch = launching;

        if(launch) 
        {
            if(checkConsecutiveStepback)
            {
                //UnityEngine.Debug.Log("revenge");
                StopCoroutine(checkCoroutine);
                consecutiveStepback = 0;
                checkConsecutiveStepback = false;
            }

            states.AttackState();
            canLaunch = false;
            StartCoroutine(WaitForLaunch());
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
        return stepback; 
    }

    public bool IsLaunching()
    {
        return launch;
    }

    public bool IsSteppingBack()
    {
        return stepback;
    }

    IEnumerator WaitForStepback()
    {
        yield return new WaitForSeconds(timeForStepback);

        Vector3 sbDir = (transform.position - chase.TargetPosition()).normalized;

        stepbackStartingPos = transform.position;

        checkStepbackDistance = true;

        //Debug.LogWarning("Accelerate");

        float time = 0;                                                                                   //Acceleration

        while (time < stepbackAccelerateTime)
        {
            rb.linearVelocity = Vector3.Lerp(Vector3.zero, sbDir * stepbackSpeed, time / stepbackAccelerateTime);
            time += Time.deltaTime;
            yield return null;
        }

        //Debug.LogWarning("Constant");
        time = 0;

        while (time < stepbackConstantTime)                                                                       //Time in constant speed
        {
            rb.linearVelocity = sbDir * stepbackSpeed;
            time += Time.deltaTime;
            yield return null;
        }

        //Debug.LogWarning("Decelerate");
        time = 0;
        Vector3 startSpeed = rb.linearVelocity;
        while (time < stepbackDecelerateTime)                                                                       //Deceleration
        {
            rb.linearVelocity = Vector3.Lerp(startSpeed, Vector3.zero, time / stepbackDecelerateTime);
            time += Time.deltaTime;
            yield return null;
        }
        //rb.AddForce(sbDir * stepbackForce);

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
        yield return new WaitForSeconds(timeForLaunch);

        launchDir = (chase.TargetPosition() - transform.position).normalized;

        launchStartingPos = transform.position;
        
        checkLaunchDistance = true;

        //Debug.LogWarning("Accelerate");

        float time = 0;                                                                                   //Acceleration

        while (time < launchAccelerateTime)
        {
            rb.linearVelocity = Vector3.Lerp(Vector3.zero, launchDir * launchSpeed, time / launchAccelerateTime);
            time += Time.deltaTime;
            yield return null;
        }

        //Debug.LogWarning("Constant");
        time = 0;

        while (time < launchConstantTime)                                                                       //Time in constant speed
        {
            rb.linearVelocity = launchDir * launchSpeed;
            time += Time.deltaTime;
            yield return null;
        }

        //Debug.LogWarning("Decelerate");
        time = 0;
        Vector3 startSpeed = rb.linearVelocity;
        while (time < launchDecelerateTime)                                                                       //Deceleration
        {
            rb.linearVelocity = Vector3.Lerp(startSpeed, Vector3.zero, time / launchDecelerateTime);
            time += Time.deltaTime;
            yield return null;
        }

        //Debug.LogWarning("Stop launch");
        //rb.AddForce(launchDir * launchForce);

        consecutiveStepback = 0;

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
        checkLaunchDistance = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        SetLaunch(false);
        StartCoroutine(TimeForLaunchReset());
    }

    public void StopStepback()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        checkStepbackDistance = false;
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
}
