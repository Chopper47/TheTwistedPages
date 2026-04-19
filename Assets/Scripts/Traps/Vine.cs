using UnityEngine;
using System.Collections;

public class VineTrap : Trap
{
    public Transform wallA;
    public Transform wallB;

    [Header("Settings")]
    public float waitTime = 5.0f;
    public float moveSpeed = 3.0f;

    private Transform currentTarget;

    [Header("Rotation")]
    public Vector3 rotationOnWall = new Vector3(0, 0, 0);
    public Vector3 rotationOnFloor = new Vector3(90, 0, 0);

    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    public AudioSource Main;
    public Animator animator;
    private void Start()
    {
        transform.position = wallA.position;
        transform.rotation = wallA.rotation * Quaternion.Euler(rotationOnWall);

        currentTarget = wallB;
        StartCoroutine(VineCycle());
        AM = FindFirstObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();
        Main.volume = 0;
    }

    private IEnumerator VineCycle()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            animator.SetTrigger("down");

            float timer = 0f;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
            
            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, newPos, timer / 0.3f);
                yield return null;
            }
            
            if(currentTarget == wallA)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
            }

            newPos += new Vector3(0, 2f, 0);
            timer = 0f;
            bool walk = true;
            animator.SetTrigger("ground");
            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, newPos, timer / 0.3f);
                if(timer > 0.2f && walk)
                {
                    animator.SetTrigger("walk");
                    walk = false;
                }
                yield return null;
            }

            //yield return new WaitForSeconds(0.2f);

            //transform.rotation = currentTarget.rotation * Quaternion.Euler(rotationOnFloor);
            yield return StartCoroutine(MoveToTarget(currentTarget));

            //transform.rotation = currentTarget.rotation * Quaternion.Euler(rotationOnWall);

            if (currentTarget == wallA)
            {
                currentTarget = wallB;
            }
            else
            {
                currentTarget = wallA;
            }
        }
    }

    private IEnumerator MoveToTarget(Transform target)
    {
        AM.Play("Vines", Main);
        animator.SetTrigger("walk");
        while (Vector3.Distance(transform.position, target.position) > 2.5f)
        {    
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        animator.SetTrigger("climb");
        while (Vector3.Distance(transform.position,target.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }   
        transform.position = target.position;
    }
}
