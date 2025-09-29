using System.Collections;
using UnityEngine;

public class WolfIA : MonoBehaviour
{
    [SerializeField] float timeForLaunch = .8f;
    [SerializeField] float launchDistance = 5f;
    [SerializeField] float launchForce = 2f;
    
    bool launch;
    bool checkLaunchDistance = false;

    Vector3 startingPos;

    Rigidbody rb;
    ChaseAgent chase;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        chase = GetComponent<ChaseAgent>();
    }

    public void Update()
    {
        if (checkLaunchDistance) 
        { 
            if(Vector3.Distance(startingPos, transform.position) >= launchDistance)
            {
                rb.linearVelocity = Vector3.zero;
                checkLaunchDistance = false;
                SetLaunch(false);
            }
        }
    }

    public void SetLaunch(bool launching)
    {
        launch = launching;

        if(launch) 
        {
            StartCoroutine(WaitForLaunch());
        }
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

}
