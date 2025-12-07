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
    private void Start()
    {
        transform.position = wallA.position;
        transform.rotation = Quaternion.Euler(rotationOnWall);

        currentTarget = wallB;
        StartCoroutine(VineCycle());
    }

    private IEnumerator VineCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            transform.rotation = Quaternion.Euler(rotationOnFloor);
            yield return StartCoroutine(MoveToTarget(currentTarget));

            transform.rotation = Quaternion.Euler(rotationOnWall);

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
        //AM.Play("Vines", Main);
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target.position;
    }
}
