using System.Collections;
using UnityEngine;

public class WolfIA : CollisionRefresh 
{
    [SerializeField] float timeForLaunch = .8f;
    [SerializeField] float launchDistance = 5f;
    [SerializeField] float launchForce = 2f;
    [SerializeField] float launchCD = 1.5f;
    
    bool launch = false;
    bool canLaunch = true;
    bool checkLaunchDistance = false;

    Vector3 startingPos;

    Rigidbody rb;
    [SerializeField] ChaseAgent chase;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (checkLaunchDistance) 
        { 
            if(Vector3.Distance(startingPos, transform.position) >= launchDistance)
            {
                StopLaunch();
            }
        }
    }

    public void SetLaunch(bool launching)
    {
        launch = launching;

        if(launch) 
        {
            canLaunch = false;
            StartCoroutine(WaitForLaunch());
        }
    }

    public bool CanLaunch()
    {
        return canLaunch;
    }

    public bool IsLaunching()
    {
        return launch;
    }

    IEnumerator WaitForLaunch()
    {
        yield return new WaitForSeconds(timeForLaunch);

        Vector3 dir = chase.TargetPosition() - transform.position;

        startingPos = transform.position;
        
        checkLaunchDistance = true;

        rb.AddForce(dir * launchForce);

    }

    /// <summary>
    /// Cooldown for the launch
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeForReset()
    {
        yield return new WaitForSeconds(launchCD);
        canLaunch = true;
    }

    /// <summary>
    /// Reset function for the launch
    /// </summary>
    public void StopLaunch()
    {
        rb.linearVelocity = Vector3.zero;
        checkLaunchDistance = false;
        SetLaunch(false);
        StartCoroutine(TimeForReset());
    }


    /// <summary>
    /// Calls the method for reset if it colides with the player
    /// </summary>
    public override void OnPlayerHit()
    {
        if(launch)
        {
            StopLaunch();
        }
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
    }

    public override void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        Debug.Log(collision.gameObject.name + " | " + player);

        if(player != null ) { OnPlayerHit(); }
    }
}
